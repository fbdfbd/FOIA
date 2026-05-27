using OneMoreSpoon.App.Encyclopedia;
using OneMoreSpoon.App.Rewards;
using OneMoreSpoon.Game.Components;
using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using System.Collections.Generic;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class MergeSystem
    {
        private const float MergeDelay = 1.5f;
        private static readonly Vector2 StackOffset = new(0f, -0.9f);
        private static readonly Vector2 StackSpacing = new(0.55f, 0f);
        private static readonly Vector2 EjectOffset = new(0f, -1.4f);
        private static readonly Vector2 ResultOffset = new(0f, 0.9f);
        private static readonly Vector2 MergeRecipeRewardOffset = new(0f, -0.6f);

        private readonly GameWorld world;
        private readonly SubstanceStackSystem stackSystem;
        private readonly MergeRecipeRegistry recipeRegistry;
        private readonly SubstanceDefinitionRegistry substanceDefinitionRegistry;
        private readonly DiscoveryService discoveryService;
        private readonly FirstDiscoveryRewardService firstDiscoveryRewardService;
        private readonly List<GameEntityId> mergeNodeBuffer = new();
        private readonly List<GameEntityId> remainingStackBuffer = new();
        private readonly List<string> inputSubstanceBuffer = new();

        public MergeSystem(
            GameWorld world,
            SubstanceStackSystem stackSystem,
            MergeRecipeRegistry recipeRegistry,
            SubstanceDefinitionRegistry substanceDefinitionRegistry,
            DiscoveryService discoveryService,
            FirstDiscoveryRewardService firstDiscoveryRewardService)
        {
            this.world = world;
            this.stackSystem = stackSystem;
            this.recipeRegistry = recipeRegistry;
            this.substanceDefinitionRegistry = substanceDefinitionRegistry;
            this.discoveryService = discoveryService;
            this.firstDiscoveryRewardService = firstDiscoveryRewardService;
        }

        public void Tick(float deltaTime)
        {
            mergeNodeBuffer.Clear();

            foreach (var pair in world.MergeSlots)
                mergeNodeBuffer.Add(pair.Key);

            foreach (var mergeNodeId in mergeNodeBuffer)
                TickMergeSlot(mergeNodeId, deltaTime);
        }

        public bool TryAddStack(GameEntityId mergeNodeId, GameEntityId stackId)
        {
            if (!world.Nodes.TryGetValue(mergeNodeId, out var node) ||
                node.Category != NodeCategory.Merge)
            {
                Debug.LogWarning($"[Merge] AddRejected mergeNode={mergeNodeId} stack={stackId} reason=TargetIsNotMergeNode");
                return false;
            }

            if (!world.SubstanceStacks.TryGetValue(stackId, out var stack))
            {
                Debug.LogWarning($"[Merge] AddRejected mergeNode={mergeNodeId} stack={stackId} reason=StackNotFound");
                return false;
            }

            if (!substanceDefinitionRegistry.TryGet(stack.SubstanceId, out var definition))
            {
                Debug.LogWarning($"[Merge] AddRejected mergeNode={mergeNodeId} stack={stackId} substance={stack.SubstanceId} reason=SubstanceDefinitionNotFound");
                return false;
            }

            if (!CanAddToMerge(definition.Kind))
            {
                Debug.LogWarning($"[Merge] AddRejected mergeNode={mergeNodeId} stack={stackId} substance={stack.SubstanceId} reason=SubstanceCannotMerge kind={definition.Kind}");
                return false;
            }

            if (!world.MergeSlots.TryGetValue(mergeNodeId, out var slot))
            {
                slot = new MergeSlotComponent();
                world.MergeSlots[mergeNodeId] = slot;
            }

            slot.AddStack(stackId, MergeDelay);
            MoveStackToSlotPosition(mergeNodeId, stackId, slot.StackIds.Count - 1);

            Debug.Log($"[Merge] StackAdded mergeNode={mergeNodeId} stack={stackId} count={slot.StackIds.Count}");
            return true;
        }

        private static bool CanAddToMerge(SubstanceKind kind)
        {
            return SubstanceKindRules.CanMerge(kind);
        }

        private void TickMergeSlot(GameEntityId mergeNodeId, float deltaTime)
        {
            if (!world.MergeSlots.TryGetValue(mergeNodeId, out var slot))
                return;

            if (!slot.IsDirty)
                return;

            slot.TimeUntilResolve -= deltaTime;

            if (slot.TimeUntilResolve > 0f)
                return;

            TryResolve(mergeNodeId, slot);
        }

        private void TryResolve(GameEntityId mergeNodeId, MergeSlotComponent slot)
        {
            RemoveMissingStacks(slot);

            if (!slot.HasEnoughInputs)
            {
                slot.MarkResolved();
                return;
            }

            if (!TryBuildInputList(slot, inputSubstanceBuffer))
            {
                slot.MarkResolved();
                return;
            }

            if (!recipeRegistry.TryGetMatch(inputSubstanceBuffer, out var recipe))
            {
                Debug.Log($"[Merge] RecipeMissing mergeNode={mergeNodeId} inputs=[{string.Join(", ", inputSubstanceBuffer)}]");
                EjectStacks(mergeNodeId, slot);
                return;
            }

            ResolveRecipe(mergeNodeId, slot, recipe);
        }

        private void ResolveRecipe(
            GameEntityId mergeNodeId,
            MergeSlotComponent slot,
            SO_MergeRecipeDefinition recipe)
        {
            remainingStackBuffer.Clear();

            foreach (var stackId in slot.StackIds)
            {
                if (!stackSystem.TryConsume(stackId))
                    continue;

                if (stackSystem.IsEmpty(stackId))
                {
                    stackSystem.Remove(stackId);
                    continue;
                }

                remainingStackBuffer.Add(stackId);
            }

            slot.StackIds.Clear();
            slot.StackIds.AddRange(remainingStackBuffer);
            slot.MarkResolved();

            for (int i = 0; i < slot.StackIds.Count; i++)
                MoveStackToSlotPosition(mergeNodeId, slot.StackIds[i], i);

            Vector2 resultPosition = GetMergeNodePosition(mergeNodeId) + ResultOffset;
            discoveryService.NotifyEncountered(recipe.ResultSubstance.SubstanceId);
            var resultStackId = world.CreateSubstanceStack(
                recipe.ResultSubstance.SubstanceId,
                recipe.ResultAmount,
                false,
                resultPosition);

            Debug.Log($"[Merge] RecipeResolved mergeNode={mergeNodeId} recipe={recipe.RecipeId} result={recipe.ResultSubstance.SubstanceId} amount={recipe.ResultAmount} stack={resultStackId}");
            firstDiscoveryRewardService.GrantForSubstance(recipe.ResultSubstance.SubstanceId, resultPosition);
            firstDiscoveryRewardService.GrantForMergeRecipe(recipe.RecipeId, resultPosition + MergeRecipeRewardOffset);
        }

        private bool TryBuildInputList(
            MergeSlotComponent slot,
            List<string> inputSubstanceIds)
        {
            inputSubstanceIds.Clear();

            foreach (var stackId in slot.StackIds)
            {
                if (!world.SubstanceStacks.TryGetValue(stackId, out var stack))
                    return false;

                inputSubstanceIds.Add(stack.SubstanceId);
            }

            return true;
        }

        private void RemoveMissingStacks(MergeSlotComponent slot)
        {
            for (int i = slot.StackIds.Count - 1; i >= 0; i--)
            {
                if (!world.SubstanceStacks.ContainsKey(slot.StackIds[i]))
                    slot.StackIds.RemoveAt(i);
            }
        }

        private void MoveStackToSlotPosition(
            GameEntityId mergeNodeId,
            GameEntityId stackId,
            int slotIndex)
        {
            Vector2 position = GetMergeNodePosition(mergeNodeId)
                + StackOffset
                + StackSpacing * slotIndex;

            stackSystem.TryMove(stackId, position);
        }

        private void EjectStacks(GameEntityId mergeNodeId, MergeSlotComponent slot)
        {
            remainingStackBuffer.Clear();
            remainingStackBuffer.AddRange(slot.StackIds);

            slot.Clear();

            for (int i = 0; i < remainingStackBuffer.Count; i++)
            {
                Vector2 position = GetMergeNodePosition(mergeNodeId)
                    + EjectOffset
                    + StackSpacing * i;

                stackSystem.TryMove(remainingStackBuffer[i], position);
            }
        }

        private Vector2 GetMergeNodePosition(GameEntityId mergeNodeId)
        {
            if (!world.Positions.TryGetValue(mergeNodeId, out var position))
                return Vector2.zero;

            return position.Value;
        }
    }
}
