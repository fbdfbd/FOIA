using OneMoreSpoon.Game.Core;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class EdgeDeleteSystem
    {
        private readonly GameWorld world;
        private readonly EdgeBlockReturnSystem edgeBlockReturnSystem;

        public EdgeDeleteSystem(
            GameWorld world,
            EdgeBlockReturnSystem edgeBlockReturnSystem)
        {
            this.world = world;
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

            return true;
        }
    }
}
