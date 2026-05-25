using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Systems;
using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Factories;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Presenter
{
    public sealed class SubstanceViewSyncSystem : ITickable
    {
        private readonly GameWorld world;
        private readonly SubstanceDockSystem dockSystem;
        private readonly SubstanceViewFactory substanceViewFactory;
        private readonly ViewRegistry viewRegistry;
        private readonly HashSet<GameEntityId> knownStackIds = new();
        private readonly List<GameEntityId> removeBuffer = new();

        public SubstanceViewSyncSystem(
            GameWorld world,
            SubstanceDockSystem dockSystem,
            SubstanceViewFactory substanceViewFactory,
            ViewRegistry viewRegistry)
        {
            this.world = world;
            this.dockSystem = dockSystem;
            this.substanceViewFactory = substanceViewFactory;
            this.viewRegistry = viewRegistry;
        }

        public void Tick()
        {
            dockSystem.SyncNewEligibleStacks();
            CreateMissingViews();
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
    }
}
