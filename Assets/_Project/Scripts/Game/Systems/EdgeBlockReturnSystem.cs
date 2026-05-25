using OneMoreSpoon.Game.Core;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class EdgeBlockReturnSystem
    {
        private readonly GameWorld world;

        public EdgeBlockReturnSystem(GameWorld world)
        {
            this.world = world;
        }

        public bool TryReturn(GameEntityId edgeId)
        {
            if (!world.EdgeBlockSlots.TryGetValue(edgeId, out var slot))
                return false;

            if (!slot.HasBlock)
                return false;

            if (!TryGetReturnPosition(edgeId, out var position))
                return false;

            world.CreateSubstanceStack(slot.EquippedSubstanceId, 1, false, position);

            slot.EquippedSubstanceId = string.Empty;
            world.EdgeBlockSlots[edgeId] = slot;

            Debug.Log($"[EdgeBlockReturn] Returned edge={edgeId}");
            return true;
        }

        private bool TryGetReturnPosition(GameEntityId edgeId, out Vector2 position)
        {
            position = Vector2.zero;

            if (!world.Edges.TryGetValue(edgeId, out var edge))
                return false;

            if (!world.Positions.TryGetValue(edge.FromNodeId, out var from))
                return false;

            if (!world.Positions.TryGetValue(edge.ToNodeId, out var to))
                return false;

            position = (from.Value + to.Value) * 0.5f;
            return true;
        }
    }
}
