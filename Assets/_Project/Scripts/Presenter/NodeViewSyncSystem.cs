using OneMoreSpoon.Game.Core;
using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Nodes;
using VContainer.Unity;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Presenter
{
    public sealed class NodeViewSyncSystem : ITickable
    {
        private readonly GameWorld world;
        private readonly GameWorldChanges changes;
        private readonly ViewRegistry viewRegistry;

        public NodeViewSyncSystem(
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
            foreach (GameEntityId entityId in changes.PositionChanged)
                RenderNodePosition(entityId);
        }

        private void RenderNodePosition(GameEntityId nodeId)
        {
            if (!world.Nodes.ContainsKey(nodeId))
                return;

            if (!world.Positions.TryGetValue(nodeId, out var position))
                return;

            if (!viewRegistry.TryGetView(nodeId, out EntityView entityView))
                return;

            if (entityView is not NodeView nodeView)
                return;

            nodeView.RenderPosition(nodeView.GetRenderPosition(position.Value), true);
        }
    }
}
