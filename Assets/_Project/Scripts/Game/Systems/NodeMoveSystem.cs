using OneMoreSpoon.Game.Components;
using OneMoreSpoon.Game.Core;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class NodeMoveSystem
    {
        private readonly GameWorld world;
        private readonly GameWorldChanges changes;
        private readonly PlacementRuleSystem placementRuleSystem;

        public NodeMoveSystem(
            GameWorld world,
            GameWorldChanges changes,
            PlacementRuleSystem placementRuleSystem)
        {
            this.world = world;
            this.changes = changes;
            this.placementRuleSystem = placementRuleSystem;
        }

        public bool TryMoveNode(GameEntityId nodeId, Vector2 targetPosition)
        {
            if (!placementRuleSystem.CanMoveTo(nodeId, targetPosition))
                return false;

            Vector2 clampedPosition = placementRuleSystem.ClampNodePosition(targetPosition);
            world.Positions[nodeId] = new PositionComponent(clampedPosition);
            changes.MarkPositionChanged(nodeId);
            return true;
        }
    }
}
