using OneMoreSpoon.Game.Components;
using OneMoreSpoon.Game.Core;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class NodeMoveSystem
    {
        private readonly GameWorld world;
        private readonly PlacementRuleSystem placementRuleSystem;

        public NodeMoveSystem(
            GameWorld world,
            PlacementRuleSystem placementRuleSystem)
        {
            this.world = world;
            this.placementRuleSystem = placementRuleSystem;
        }

        public bool TryMoveNode(GameEntityId nodeId, Vector2 targetPosition)
        {
            if (!placementRuleSystem.CanMoveTo(nodeId, targetPosition))
                return false;

            world.Positions[nodeId] = new PositionComponent(targetPosition);
            return true;
        }
    }
}