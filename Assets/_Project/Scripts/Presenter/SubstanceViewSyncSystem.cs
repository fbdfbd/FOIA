using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Systems;
using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Factories;
using OneMoreSpoon.View.Substances;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Presenter
{
    public sealed class SubstanceViewSyncSystem : ITickable
    {
        private readonly GameWorld world;
        private readonly GameWorldChanges changes;
        private readonly SubstanceDockSystem dockSystem;
        private readonly SubstanceViewFactory substanceViewFactory;
        private readonly ViewRegistry viewRegistry;
        private readonly HashSet<GameEntityId> knownStackIds = new();
        private readonly List<GameEntityId> removeBuffer = new();

        public SubstanceViewSyncSystem(
            GameWorld world,
            GameWorldChanges changes,
            SubstanceDockSystem dockSystem,
            SubstanceViewFactory substanceViewFactory,
            ViewRegistry viewRegistry)
        {
            this.world = world;
            this.changes = changes;
            this.dockSystem = dockSystem;
            this.substanceViewFactory = substanceViewFactory;
            this.viewRegistry = viewRegistry;
        }

        public void Tick()
        {
            dockSystem.SyncNewEligibleStacks();
            CreateMissingViews();
            RenderChangedViews();
            RemoveStaleViews();
        }

        private void CreateMissingViews()
        {
            foreach (var pair in world.SubstanceStacks)
            {
                if (viewRegistry.TryGetView(pair.Key, out _))
                    continue;

                substanceViewFactory.Create(pair.Key);
                knownStackIds.Add(pair.Key);
            }
        }

        private void RemoveStaleViews()
        {
            removeBuffer.Clear();

            foreach (var stackId in knownStackIds)
            {
                if (!world.SubstanceStacks.ContainsKey(stackId))
                    removeBuffer.Add(stackId);
            }

            foreach (var stackId in removeBuffer)
                RemoveView(stackId);
        }

        private void RemoveView(GameEntityId stackId)
        {
            knownStackIds.Remove(stackId);

            if (!viewRegistry.TryGetView(stackId, out var view))
                return;

            viewRegistry.Unregister(stackId);
            Object.Destroy(view.gameObject);
        }

        private void RenderChangedViews()
        {
            foreach (var stackId in changes.PositionChanged)
                RenderStackPosition(stackId);

            foreach (var stackId in changes.SubstanceStackChanged)
                RenderStackText(stackId);
        }

        private void RenderStackPosition(GameEntityId stackId)
        {
            if (!world.SubstanceStacks.ContainsKey(stackId))
                return;

            if (!world.Positions.TryGetValue(stackId, out var position))
                return;

            if (!viewRegistry.TryGetView(stackId, out EntityView entityView))
                return;

            if (entityView is SubstanceView substanceView)
                substanceView.RenderPosition(substanceView.GetRenderPosition(position.Value), true);
        }

        private void RenderStackText(GameEntityId stackId)
        {
            if (!viewRegistry.TryGetView(stackId, out EntityView entityView))
                return;

            if (entityView is SubstanceView substanceView)
                substanceView.RefreshTextFromWorld();
        }
    }
}
