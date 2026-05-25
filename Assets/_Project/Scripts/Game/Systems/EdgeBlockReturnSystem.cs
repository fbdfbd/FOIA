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

            foreach (var substanceId in slot.EquippedSubstanceIds)
                world.CreateSubstanceStack(substanceId, 1, false, position);

            var returnedCount = slot.EquippedSubstanceIds.Count;
            slot.Clear();
            world.EdgeBlockSlots[edgeId] = slot;

            Debug.Log($"[EdgeBlockReturn] Returned edge={edgeId} count={returnedCount}");
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
