using OneMoreSpoon.Game.Core;
using OneMoreSpoon.View.Edges;
using OneMoreSpoon.View.Factories;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Presenter
{
    public sealed class EdgeBlockIndicatorSyncSystem : ITickable
    {
        private readonly GameWorld world;
        private readonly EdgeBlockIndicatorViewFactory indicatorViewFactory;
        private readonly Dictionary<GameEntityId, EdgeBlockIndicatorView> indicatorViews = new();
        private readonly List<GameEntityId> removeBuffer = new();

        public EdgeBlockIndicatorSyncSystem(
            GameWorld world,
            EdgeBlockIndicatorViewFactory indicatorViewFactory)
        {
            this.world = world;
            this.indicatorViewFactory = indicatorViewFactory;
        }

        public void Tick()
        {
            CreateMissingViews();
            RemoveStaleViews();
        }

        private void CreateMissingViews()
        {
            foreach (var pair in world.EdgeBlockSlots)
            {
                if (!pair.Value.HasBlock)
                    continue;

                if (!world.Edges.ContainsKey(pair.Key))
                    continue;

                if (indicatorViews.ContainsKey(pair.Key))
                    continue;

                var view = indicatorViewFactory.Create(pair.Key);

                if (view != null)
                    indicatorViews.Add(pair.Key, view);
            }
        }

        private void RemoveStaleViews()
        {
            removeBuffer.Clear();

            foreach (var pair in indicatorViews)
            {
                if (!ShouldKeep(pair.Key, pair.Value))
                    removeBuffer.Add(pair.Key);
            }

            foreach (var edgeId in removeBuffer)
                RemoveView(edgeId);
        }

        private bool ShouldKeep(GameEntityId edgeId, EdgeBlockIndicatorView view)
        {
            if (view == null)
                return false;

            if (!world.Edges.ContainsKey(edgeId))
                return false;

            return world.EdgeBlockSlots.TryGetValue(edgeId, out var slot) && slot.HasBlock;
        }

        private void RemoveView(GameEntityId edgeId)
        {
            if (!indicatorViews.TryGetValue(edgeId, out var view))
                return;

            indicatorViews.Remove(edgeId);

            if (view != null)
                Object.Destroy(view.gameObject);
        }
    }
}
