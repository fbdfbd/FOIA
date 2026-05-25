using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Nodes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer.Unity;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Presenter
{
    public sealed class MergeSlotTextSyncSystem : ITickable
    {
        private readonly GameWorld world;
        private readonly ViewRegistry viewRegistry;
        private readonly SubstanceDefinitionRegistry substanceDefinitions;
        private readonly HashSet<GameEntityId> hiddenStacks = new();
        private readonly List<GameEntityId> visibleBuffer = new();

        public MergeSlotTextSyncSystem(
            GameWorld world,
            ViewRegistry viewRegistry,
            SubstanceDefinitionRegistry substanceDefinitions)
        {
            this.world = world;
            this.viewRegistry = viewRegistry;
            this.substanceDefinitions = substanceDefinitions;
        }

        public void Tick()
        {
            visibleBuffer.Clear();

            foreach (GameEntityId stackId in hiddenStacks)
                visibleBuffer.Add(stackId);

            foreach (var pair in world.MergeSlots)
            {
                GameEntityId mergeNodeId = pair.Key;
                var slot = pair.Value;

                if (!viewRegistry.TryGetView(mergeNodeId, out EntityView nodeView))
                    continue;

                MergeNodeSlotView slotView = nodeView.GetComponent<MergeNodeSlotView>();

                if (slotView == null)
                    continue;

                SyncSlot(slot, slotView, 0);
                SyncSlot(slot, slotView, 1);
            }

            foreach (GameEntityId stackId in visibleBuffer)
                ShowStack(stackId);
        }

        private void SyncSlot(
            OneMoreSpoon.Game.Components.MergeSlotComponent slot,
            MergeNodeSlotView slotView,
            int index)
        {
            if (slot.StackIds.Count <= index)
            {
                slotView.SetSlotText(index, string.Empty);
                return;
            }

            GameEntityId stackId = slot.StackIds[index];

            if (!world.SubstanceStacks.TryGetValue(stackId, out var stack))
            {
                slotView.SetSlotText(index, string.Empty);
                return;
            }

            slotView.SetSlotText(index, GetDisplayName(stack.SubstanceId));
            HideStack(stackId);
            visibleBuffer.Remove(stackId);
        }

        private string GetDisplayName(string substanceId)
        {
            if (substanceDefinitions.TryGet(substanceId, out SO_SubstanceDefinition definition))
                return definition.DisplayName;

            return substanceId;
        }

        private void HideStack(GameEntityId stackId)
        {
            if (!viewRegistry.TryGetView(stackId, out EntityView view))
                return;

            SetVisible(view.gameObject, false);
            hiddenStacks.Add(stackId);
        }

        private void ShowStack(GameEntityId stackId)
        {
            if (!viewRegistry.TryGetView(stackId, out EntityView view))
                return;

            SetVisible(view.gameObject, true);
            hiddenStacks.Remove(stackId);
        }

        private void SetVisible(GameObject target, bool visible)
        {
            foreach (Renderer renderer in target.GetComponentsInChildren<Renderer>())
                renderer.enabled = visible;

            foreach (Collider2D collider in target.GetComponentsInChildren<Collider2D>())
                collider.enabled = visible;

            foreach (Canvas canvas in target.GetComponentsInChildren<Canvas>())
                canvas.enabled = visible;

            foreach (Graphic graphic in target.GetComponentsInChildren<Graphic>())
                graphic.enabled = visible;
        }
    }
}
