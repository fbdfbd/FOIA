using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Components;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.View.Substances;
using System.Collections.Generic;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class SubstanceStackSystem
    {
        private readonly GameWorld world;
        private readonly PlayAreaBoundsSystem playAreaBounds;

        public SubstanceStackSystem(GameWorld world, PlayAreaBoundsSystem playAreaBounds)
        {
            this.world = world;
            this.playAreaBounds = playAreaBounds;
        }

        public bool TryConsume(GameEntityId stackId)
        {
            return TryConsume(stackId, out _);
        }

        public bool TryConsume(GameEntityId stackId, out bool removed)
        {
            if (!world.SubstanceStacks.TryGetValue(stackId, out var stack))
            {
                removed = false;
                return false;
            }

            if (stack.IsInfinite)
            {
                removed = false;
                return true;
            }

            if (stack.Amount <= 0)
            {
                Remove(stackId);
                removed = true;
                return false;
            }

            stack.Amount--;
            world.SubstanceStacks[stackId] = stack;

            if (stack.IsEmpty)
            {
                Remove(stackId);
                removed = true;
                return true;
            }

            removed = false;
            return true;
        }

        public bool CanConsume(GameEntityId stackId)
        {
            if (!world.SubstanceStacks.TryGetValue(stackId, out var stack))
                return false;

            return stack.IsInfinite || stack.Amount > 0;
        }

        public bool IsEmpty(GameEntityId stackId)
        {
            return world.SubstanceStacks.TryGetValue(stackId, out var stack) && stack.IsEmpty;
        }

        public void Remove(GameEntityId stackId)
        {
            world.SubstanceStacks.Remove(stackId);
            world.Positions.Remove(stackId);
        }

        public bool TryMove(GameEntityId stackId, Vector2 position)
        {
            if (!world.SubstanceStacks.ContainsKey(stackId))
                return false;

            Vector2 clampedPosition = playAreaBounds.Clamp(position);
            world.Positions[stackId] = new PositionComponent(clampedPosition);
            return true;
        }

        public bool TryMoveUnclamped(GameEntityId stackId, Vector2 position)
        {
            if (!world.SubstanceStacks.ContainsKey(stackId))
                return false;

            world.Positions[stackId] = new PositionComponent(position);
            return true;
        }
    }

    public sealed class SubstanceStackSpawnService
    {
        private readonly GameWorld world;
        private readonly SubstanceDefinitionRegistry substanceDefinitions;
        private readonly List<GameEntityId> matchingPersonStackIds = new();
        private readonly List<GameEntityId> matchingStackIds = new();

        public SubstanceStackSpawnService(
            GameWorld world,
            SubstanceDefinitionRegistry substanceDefinitions)
        {
            this.world = world;
            this.substanceDefinitions = substanceDefinitions;
        }

        public GameEntityId CreateOrTransitionStack(
            SO_SubstanceDefinition definition,
            int amount,
            bool isInfinite,
            Vector2 position)
        {
            if (definition == null)
                return GameEntityId.Invalid;

            if (!IsStatefulPerson(definition.Kind) ||
                !PersonSubstanceIdentityParser.TryParse(definition.SubstanceId, out var targetIdentity))
                return CreateOrStackBySubstanceId(definition.SubstanceId, amount, isInfinite, position);

            if (!TryFindExistingPersonStack(targetIdentity.CharacterKey, out var stackId))
            {
                return world.CreateSubstanceStack(
                    definition.SubstanceId,
                    Mathf.Max(0, amount),
                    isInfinite,
                    position);
            }

            var existingStack = world.SubstanceStacks[stackId];
            var resultIsInfinite = existingStack.IsInfinite || isInfinite;
            var resultAmount = resultIsInfinite
                ? Mathf.Max(existingStack.Amount, amount)
                : existingStack.Amount + Mathf.Max(0, amount);

            world.SubstanceStacks[stackId] = new SubstanceStackComponent(
                definition.SubstanceId,
                resultAmount,
                resultIsInfinite);
            world.Positions[stackId] = new PositionComponent(position);

            RemoveExtraPersonStacks(stackId);

            Debug.Log($"[SubstanceStack] PersonTransition character={targetIdentity.CharacterKey} state={targetIdentity.State} stack={stackId} substance={definition.SubstanceId}");
            return stackId;
        }

        private GameEntityId CreateOrStackBySubstanceId(
            string substanceId,
            int amount,
            bool isInfinite,
            Vector2 position)
        {
            if (!TryFindExistingStack(substanceId, out var stackId))
            {
                return world.CreateSubstanceStack(
                    substanceId,
                    Mathf.Max(0, amount),
                    isInfinite,
                    position);
            }

            AddToStack(stackId, substanceId, amount, isInfinite, position);
            return stackId;
        }

        private bool TryFindExistingStack(string substanceId, out GameEntityId stackId)
        {
            matchingStackIds.Clear();

            foreach (var pair in world.SubstanceStacks)
            {
                if (pair.Value.SubstanceId != substanceId)
                    continue;

                matchingStackIds.Add(pair.Key);
            }

            matchingStackIds.Sort((left, right) => left.Value.CompareTo(right.Value));

            if (matchingStackIds.Count <= 0)
            {
                stackId = GameEntityId.Invalid;
                return false;
            }

            stackId = matchingStackIds[0];
            return true;
        }

        private void AddToStack(
            GameEntityId stackId,
            string substanceId,
            int amount,
            bool isInfinite,
            Vector2 position)
        {
            var resultIsInfinite = isInfinite;
            var resultAmount = Mathf.Max(0, amount);

            for (var i = 0; i < matchingStackIds.Count; i++)
            {
                var matchingStackId = matchingStackIds[i];
                if (!world.SubstanceStacks.TryGetValue(matchingStackId, out var stack))
                    continue;

                resultIsInfinite |= stack.IsInfinite;
                resultAmount = resultIsInfinite
                    ? Mathf.Max(resultAmount, stack.Amount)
                    : resultAmount + Mathf.Max(0, stack.Amount);
            }

            world.SubstanceStacks[stackId] = new SubstanceStackComponent(
                substanceId,
                resultAmount,
                resultIsInfinite);
            world.Positions[stackId] = new PositionComponent(position);

            for (var i = 0; i < matchingStackIds.Count; i++)
            {
                var matchingStackId = matchingStackIds[i];
                if (matchingStackId == stackId)
                    continue;

                world.SubstanceStacks.Remove(matchingStackId);
                world.Positions.Remove(matchingStackId);
            }
        }

        public int RemovePersonStacks(string substanceId)
        {
            if (!PersonSubstanceIdentityParser.TryParse(substanceId, out var identity))
                return 0;

            matchingPersonStackIds.Clear();

            foreach (var pair in world.SubstanceStacks)
            {
                if (!IsSameCharacter(pair.Value.SubstanceId, identity.CharacterKey))
                    continue;

                matchingPersonStackIds.Add(pair.Key);
            }

            for (var i = 0; i < matchingPersonStackIds.Count; i++)
            {
                var stackId = matchingPersonStackIds[i];
                world.SubstanceStacks.Remove(stackId);
                world.Positions.Remove(stackId);
            }

            return matchingPersonStackIds.Count;
        }

        private bool TryFindExistingPersonStack(
            string characterKey,
            out GameEntityId stackId)
        {
            matchingPersonStackIds.Clear();

            foreach (var pair in world.SubstanceStacks)
            {
                if (!IsSameCharacter(pair.Value.SubstanceId, characterKey))
                    continue;

                matchingPersonStackIds.Add(pair.Key);
            }

            matchingPersonStackIds.Sort((left, right) => left.Value.CompareTo(right.Value));

            if (matchingPersonStackIds.Count <= 0)
            {
                stackId = GameEntityId.Invalid;
                return false;
            }

            stackId = matchingPersonStackIds[0];
            return true;
        }

        private void RemoveExtraPersonStacks(GameEntityId retainedStackId)
        {
            for (var i = 0; i < matchingPersonStackIds.Count; i++)
            {
                var stackId = matchingPersonStackIds[i];
                if (stackId == retainedStackId)
                    continue;

                world.SubstanceStacks.Remove(stackId);
                world.Positions.Remove(stackId);
            }
        }

        private bool IsSameCharacter(string substanceId, string characterKey)
        {
            if (!substanceDefinitions.TryGet(substanceId, out var definition))
                return false;

            return IsStatefulPerson(definition.Kind) &&
                PersonSubstanceIdentityParser.TryParse(substanceId, out var identity) &&
                identity.CharacterKey == characterKey;
        }

        private static bool IsStatefulPerson(SubstanceKind kind)
        {
            return kind == SubstanceKind.Person
                || kind == SubstanceKind.Person_Friend
                || kind == SubstanceKind.Person_Captive
                || kind == SubstanceKind.Person_Create
                || kind == SubstanceKind.Person_Replace;
        }
    }

    public enum SubstanceDockKind
    {
        Dish,
        EdgeBlock,
        TraitShard
    }

    public sealed class SubstanceDockDepthState
    {
        private readonly Dictionary<GameEntityId, float> depthsByStackId = new();

        public float GetDepth(GameEntityId stackId)
        {
            return depthsByStackId.TryGetValue(stackId, out float depth)
                ? depth
                : 0f;
        }

        public void SetDepth(GameEntityId stackId, float depth)
        {
            depthsByStackId[stackId] = depth;
        }

        public void ClearDepth(GameEntityId stackId)
        {
            depthsByStackId.Remove(stackId);
        }
    }

    [System.Serializable]
    public sealed class SubstanceDockLayoutSettings
    {
        [SerializeField] private float horizontalSpacing = 1.05f;
        [SerializeField] private float verticalSpacing = 1.05f;
        [SerializeField] private float horizontalPadding = 0.55f;
        [SerializeField] private float verticalPadding = 0.55f;
        [SerializeField] private float dockedStackZStep = -0.01f;

        public float HorizontalSpacing => horizontalSpacing;
        public float VerticalSpacing => verticalSpacing;
        public float HorizontalPadding => horizontalPadding;
        public float VerticalPadding => verticalPadding;
        public float DockedStackZStep => dockedStackZStep;
    }

    public readonly struct SubstanceDockArea
    {
        public readonly SubstanceDockKind Kind;
        public readonly string Label;
        public readonly Bounds Bounds;

        public SubstanceDockArea(SubstanceDockKind kind, string label, Bounds bounds)
        {
            Kind = kind;
            Label = label;
            Bounds = bounds;
        }

        public bool Contains(Vector2 position)
        {
            return Bounds.Contains(new Vector3(position.x, position.y, Bounds.center.z));
        }
    }

    public sealed class SubstanceDockAreaRegistry
    {
        private readonly Dictionary<SubstanceDockKind, SubstanceDockAreaView> viewsByKind = new();
        private readonly List<SubstanceDockArea> areaBuffer = new();

        public SubstanceDockAreaRegistry(
            SubstanceDockAreaView dishDockArea,
            SubstanceDockAreaView edgeBlockDockArea,
            SubstanceDockAreaView traitShardDockArea)
        {
            Register(SubstanceDockKind.Dish, dishDockArea);
            Register(SubstanceDockKind.EdgeBlock, edgeBlockDockArea);
            Register(SubstanceDockKind.TraitShard, traitShardDockArea);
        }

        public IReadOnlyList<SubstanceDockArea> Areas
        {
            get
            {
                areaBuffer.Clear();

                AddArea(SubstanceDockKind.Dish);
                AddArea(SubstanceDockKind.EdgeBlock);
                AddArea(SubstanceDockKind.TraitShard);

                return areaBuffer;
            }
        }

        public bool TryGetArea(SubstanceDockKind dockKind, out SubstanceDockArea dockArea)
        {
            if (!viewsByKind.TryGetValue(dockKind, out var view) ||
                view == null ||
                !view.isActiveAndEnabled)
            {
                dockArea = default;
                return false;
            }

            dockArea = view.ToArea();
            return true;
        }

        private void Register(SubstanceDockKind expectedDockKind, SubstanceDockAreaView view)
        {
            if (view == null)
            {
                Debug.LogWarning($"[SubstanceDock] Area reference missing kind={expectedDockKind}");
                return;
            }

            if (view.DockKind != expectedDockKind)
            {
                Debug.LogWarning($"[SubstanceDock] Area reference kind mismatch expected={expectedDockKind} actual={view.DockKind}");
                return;
            }

            viewsByKind[expectedDockKind] = view;
        }

        private void AddArea(SubstanceDockKind dockKind)
        {
            if (TryGetArea(dockKind, out var area))
                areaBuffer.Add(area);
        }
    }

    public sealed class SubstanceDockSystem
    {
        private readonly GameWorld world;
        private readonly SubstanceStackSystem stackSystem;
        private readonly SubstanceDefinitionRegistry definitionRegistry;
        private readonly SubstanceDockLayoutSettings settings;
        private readonly SubstanceDockDepthState depthState;
        private readonly SubstanceDockAreaRegistry areaRegistry;
        private readonly Dictionary<GameEntityId, SubstanceDockKind> dockedStacks = new();
        private readonly Dictionary<SubstanceDockKind, List<GameEntityId>> dockOrderByKind = new();
        private readonly HashSet<GameEntityId> knownStacks = new();
        private readonly HashSet<GameEntityId> draggingStacks = new();
        private readonly List<GameEntityId> stackBuffer = new();
        private readonly List<GameEntityId> staleBuffer = new();

        public SubstanceDockSystem(
            GameWorld world,
            SubstanceStackSystem stackSystem,
            SubstanceDefinitionRegistry definitionRegistry,
            SubstanceDockLayoutSettings settings,
            SubstanceDockDepthState depthState,
            SubstanceDockAreaRegistry areaRegistry)
        {
            this.world = world;
            this.stackSystem = stackSystem;
            this.definitionRegistry = definitionRegistry;
            this.settings = settings;
            this.depthState = depthState;
            this.areaRegistry = areaRegistry;
        }

        public IReadOnlyList<SubstanceDockArea> Areas => areaRegistry.Areas;

        public void SyncNewEligibleStacks()
        {
            RemoveStaleStacks();

            foreach (var pair in world.SubstanceStacks)
            {
                GameEntityId stackId = pair.Key;

                if (knownStacks.Contains(stackId))
                    continue;

                knownStacks.Add(stackId);

                if (!TryGetDockKind(pair.Value.SubstanceId, out SubstanceDockKind dockKind))
                    continue;

                DockInternal(stackId, dockKind);
            }

            RearrangeAll();
        }

        public void BeginDrag(GameEntityId stackId)
        {
            knownStacks.Add(stackId);
            draggingStacks.Add(stackId);

            if (!dockedStacks.TryGetValue(stackId, out SubstanceDockKind dockKind))
                return;

            dockedStacks.Remove(stackId);
            RemoveFromDockOrder(stackId, dockKind);
            depthState.ClearDepth(stackId);
            Rearrange(dockKind);
        }

        public void EndDrag(GameEntityId stackId)
        {
            draggingStacks.Remove(stackId);
        }

        public bool TryDock(GameEntityId stackId, SubstanceDockKind dockKind)
        {
            if (!world.SubstanceStacks.TryGetValue(stackId, out var stack))
                return false;

            if (!TryGetDockKind(stack.SubstanceId, out SubstanceDockKind expectedDockKind))
                return false;

            if (expectedDockKind != dockKind)
                return false;

            knownStacks.Add(stackId);
            draggingStacks.Remove(stackId);
            DockInternal(stackId, dockKind);
            Rearrange(dockKind);
            return true;
        }

        public void MarkFree(GameEntityId stackId)
        {
            knownStacks.Add(stackId);
            draggingStacks.Remove(stackId);

            if (dockedStacks.TryGetValue(stackId, out SubstanceDockKind dockKind))
            {
                dockedStacks.Remove(stackId);
                RemoveFromDockOrder(stackId, dockKind);
                Rearrange(dockKind);
            }

            depthState.ClearDepth(stackId);
        }

        public bool TryGetDockAt(Vector2 position, out SubstanceDockKind dockKind)
        {
            foreach (var area in Areas)
            {
                if (!area.Contains(position))
                    continue;

                dockKind = area.Kind;
                return true;
            }

            dockKind = default;
            return false;
        }

        public bool IsDocked(GameEntityId stackId)
        {
            return dockedStacks.ContainsKey(stackId);
        }

        private void DockInternal(GameEntityId stackId, SubstanceDockKind dockKind)
        {
            if (dockedStacks.TryGetValue(stackId, out SubstanceDockKind previousDockKind))
            {
                if (previousDockKind == dockKind)
                {
                    EnsureDockOrderContains(stackId, dockKind);
                    return;
                }

                RemoveFromDockOrder(stackId, previousDockKind);
            }

            dockedStacks[stackId] = dockKind;
            GetDockOrder(dockKind).Add(stackId);
        }

        private void RearrangeAll()
        {
            foreach (var area in Areas)
                Rearrange(area.Kind);
        }

        private void Rearrange(SubstanceDockKind dockKind)
        {
            CollectDockedStacks(dockKind);

            if (stackBuffer.Count <= 0)
                return;

            if (!areaRegistry.TryGetArea(dockKind, out SubstanceDockArea area))
            {
                Debug.LogWarning($"[SubstanceDock] Area not found kind={dockKind}");
                return;
            }

            for (int i = 0; i < stackBuffer.Count; i++)
            {
                GameEntityId stackId = stackBuffer[i];

                if (draggingStacks.Contains(stackId))
                    continue;

                stackSystem.TryMoveUnclamped(stackId, GetSlotPosition(area.Bounds, i));
                depthState.SetDepth(stackId, settings.DockedStackZStep * i);
            }
        }

        private void CollectDockedStacks(SubstanceDockKind dockKind)
        {
            stackBuffer.Clear();

            if (!dockOrderByKind.TryGetValue(dockKind, out var dockOrder))
                return;

            foreach (var stackId in dockOrder)
            {
                if (dockedStacks.TryGetValue(stackId, out SubstanceDockKind currentDockKind) &&
                    currentDockKind == dockKind &&
                    world.SubstanceStacks.ContainsKey(stackId))
                {
                    stackBuffer.Add(stackId);
                }
            }
        }

        private Vector2 GetSlotPosition(Bounds bounds, int index)
        {
            float usableWidth = Mathf.Max(0.1f, bounds.size.x - settings.HorizontalPadding * 2f);
            int columns = Mathf.Max(1, Mathf.FloorToInt(usableWidth / settings.HorizontalSpacing) + 1);

            int column = index % columns;
            int row = index / columns;

            return new Vector2(
                bounds.min.x + settings.HorizontalPadding + settings.HorizontalSpacing * column,
                bounds.max.y - settings.VerticalPadding - settings.VerticalSpacing * row);
        }

        private bool TryGetDockKind(string substanceId, out SubstanceDockKind dockKind)
        {
            if (!definitionRegistry.TryGet(substanceId, out SO_SubstanceDefinition definition))
            {
                dockKind = default;
                return false;
            }

            if (SubstanceKindRules.IsPersonLike(definition.Kind))
            {
                dockKind = SubstanceDockKind.Dish;
                return true;
            }

            if (SubstanceKindRules.IsEdgeBlockLike(definition.Kind))
            {
                dockKind = SubstanceDockKind.EdgeBlock;
                return true;
            }

            if (SubstanceKindRules.IsStanceLike(definition.Kind) ||
                SubstanceKindRules.IsEtcLike(definition.Kind))
            {
                dockKind = SubstanceDockKind.TraitShard;
                return true;
            }

            dockKind = default;
            return false;
        }

        private void RemoveStaleStacks()
        {
            staleBuffer.Clear();

            foreach (var stackId in knownStacks)
                if (!world.SubstanceStacks.ContainsKey(stackId))
                    staleBuffer.Add(stackId);

            foreach (var stackId in staleBuffer)
            {
                knownStacks.Remove(stackId);
                draggingStacks.Remove(stackId);

                if (dockedStacks.TryGetValue(stackId, out SubstanceDockKind dockKind))
                {
                    dockedStacks.Remove(stackId);
                    RemoveFromDockOrder(stackId, dockKind);
                }

                depthState.ClearDepth(stackId);
            }
        }

        private List<GameEntityId> GetDockOrder(SubstanceDockKind dockKind)
        {
            if (!dockOrderByKind.TryGetValue(dockKind, out var dockOrder))
            {
                dockOrder = new List<GameEntityId>();
                dockOrderByKind[dockKind] = dockOrder;
            }

            return dockOrder;
        }

        private void EnsureDockOrderContains(GameEntityId stackId, SubstanceDockKind dockKind)
        {
            var dockOrder = GetDockOrder(dockKind);

            if (!dockOrder.Contains(stackId))
                dockOrder.Add(stackId);
        }

        private void RemoveFromDockOrder(GameEntityId stackId, SubstanceDockKind dockKind)
        {
            if (dockOrderByKind.TryGetValue(dockKind, out var dockOrder))
                dockOrder.Remove(stackId);
        }
    }
}
