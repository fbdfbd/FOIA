using OneMoreSpoon.Game.Core;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class PlacementRuleSystem
    {
        private readonly GameWorld world;
        private readonly PlayAreaBoundsSystem playAreaBoundsSystem;

        public PlacementRuleSystem(GameWorld world, PlayAreaBoundsSystem playAreaBoundsSystem)
        {
            this.world = world;
            this.playAreaBoundsSystem = playAreaBoundsSystem;
        }

        public bool CanMoveTo(GameEntityId nodeId, Vector2 targetPosition)
        {
            if (!world.Draggables.TryGetValue(nodeId, out var draggable))
                return false;

            if (!draggable.CanDrag)
                return false;

            if (world.Tags.TryGetValue(nodeId, out var tags))
            {
                if (tags.Has("Fixed"))
                    return false;
            }

            return true;
        }

        public Vector2 ClampNodePosition(Vector2 targetPosition)
        {
            return playAreaBoundsSystem.Clamp(targetPosition);
        }

        public bool CanCreateNodeAt(Vector2 targetPosition)
        {
            return playAreaBoundsSystem.Contains(targetPosition);
        }
    }
}