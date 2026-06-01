using OneMoreSpoon.Game.Core;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class EdgeDeleteSystem
    {
        private readonly GameWorld world;
        private readonly GameWorldChanges changes;
        private readonly EdgeBlockReturnSystem edgeBlockReturnSystem;

        public EdgeDeleteSystem(
            GameWorld world,
            GameWorldChanges changes,
            EdgeBlockReturnSystem edgeBlockReturnSystem)
        {
            this.world = world;
            this.changes = changes;
            this.edgeBlockReturnSystem = edgeBlockReturnSystem;
        }

        public bool TryDeleteEdge(GameEntityId edgeId)
        {
            if (!world.Edges.ContainsKey(edgeId))
                return false;

            edgeBlockReturnSystem.TryReturn(edgeId);

            world.Edges.Remove(edgeId);
            world.EdgeStates.Remove(edgeId);
            world.EdgeBlockSlots.Remove(edgeId);
            changes.MarkEdgeChanged(edgeId);
            changes.MarkEdgeBlockChanged(edgeId);

            return true;
        }
    }
}
