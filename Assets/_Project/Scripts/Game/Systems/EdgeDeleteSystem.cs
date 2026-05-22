using OneMoreSpoon.Game.Core;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class EdgeDeleteSystem
    {
        private readonly GameWorld world;

        public EdgeDeleteSystem(GameWorld world)
        {
            this.world = world;
        }

        public bool TryDeleteEdge(GameEntityId edgeId)
        {
            if (!world.Edges.ContainsKey(edgeId))
                return false;

            world.Edges.Remove(edgeId);
            world.EdgeStates.Remove(edgeId);

            return true;
        }
    }
}