using OneMoreSpoon.Game.Core;
using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Edges;
using System.Collections.Generic;
using VContainer.Unity;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Presenter
{
    public sealed class EdgeViewSyncSystem : ITickable
    {
        private readonly GameWorld world;
        private readonly GameWorldChanges changes;
        private readonly ViewRegistry viewRegistry;
        private readonly HashSet<GameEntityId> edgesToRender = new();

        public EdgeViewSyncSystem(
            GameWorld world,
            GameWorldChanges changes,
            ViewRegistry viewRegistry)
        {
            this.world = world;
            this.changes = changes;
            this.viewRegistry = viewRegistry;
        }

        public void Tick()
        {
            edgesToRender.Clear();

            foreach (GameEntityId edgeId in changes.EdgeChanged)
                edgesToRender.Add(edgeId);

            foreach (GameEntityId movedEntityId in changes.PositionChanged)
                AddEdgesConnectedTo(movedEntityId);

            foreach (GameEntityId edgeId in edgesToRender)
                RenderEdge(edgeId);
        }

        private void AddEdgesConnectedTo(GameEntityId entityId)
        {
            if (!world.Nodes.ContainsKey(entityId))
                return;

            foreach (var pair in world.Edges)
            {
                var edge = pair.Value;

                if (edge.FromNodeId == entityId || edge.ToNodeId == entityId)
                    edgesToRender.Add(pair.Key);
            }
        }

        private void RenderEdge(GameEntityId edgeId)
        {
            if (!world.Edges.TryGetValue(edgeId, out var edge))
                return;

            if (!world.Positions.TryGetValue(edge.FromNodeId, out var fromPosition))
                return;

            if (!world.Positions.TryGetValue(edge.ToNodeId, out var toPosition))
                return;

            if (!viewRegistry.TryGetView(edgeId, out EntityView entityView))
                return;

            if (entityView is EdgeView edgeView)
                edgeView.RenderLine(edge, fromPosition.Value, toPosition.Value);
        }
    }
}
