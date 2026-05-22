using OneMoreSpoon.Game.Core;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Factories
{
    public sealed class EdgeFactory
    {
        private readonly GameWorld world;

        public EdgeFactory(GameWorld world)
        {
            this.world = world;
        }

        public bool TryCreateEdge(
            GameEntityId fromNodeId,
            GameEntityId toNodeId,
            out GameEntityId edgeId)
        {
            edgeId = GameEntityId.Invalid;

            if (!fromNodeId.IsValid || !toNodeId.IsValid)
                return false;

            if (fromNodeId == toNodeId)
                return false;

            if (!world.Nodes.ContainsKey(fromNodeId))
                return false;

            if (!world.Nodes.ContainsKey(toNodeId))
                return false;

            if (world.HasEdge(fromNodeId, toNodeId))
                return false;

            edgeId = world.CreateEdge(fromNodeId, toNodeId);

            Debug.Log($"Created edge: {fromNodeId} -> {toNodeId}");

            return true;
        }
    }
}