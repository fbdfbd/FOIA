using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Components;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class SubstanceStackSystem
    {
        private readonly GameWorld world;

        public SubstanceStackSystem(GameWorld world)
        {
            this.world = world;
        }

        public bool TryConsume(GameEntityId stackId)
        {
            if (!world.SubstanceStacks.TryGetValue(stackId, out var stack))
                return false;

            if (stack.IsInfinite)
                return true;

            if (stack.Amount <= 0)
                return false;

            stack.Amount--;
            world.SubstanceStacks[stackId] = stack;

            return true;
        }

        public bool CanConsume(GameEntityId stackId)
        {
            if (!world.SubstanceStacks.TryGetValue(stackId, out var stack))
                return false;

            return stack.IsInfinite || stack.Amount > 0;
        }

        public bool IsEmpty(GameEntityId stackId)
        {
            return world.SubstanceStacks.TryGetValue(stackId, out var stack) && stack.IsEmpty;
        }

        public void Remove(GameEntityId stackId)
        {
            world.SubstanceStacks.Remove(stackId);
            world.Positions.Remove(stackId);
        }

        public bool TryMove(GameEntityId stackId, Vector2 position)
        {
            if (!world.SubstanceStacks.ContainsKey(stackId))
                return false;

            world.Positions[stackId] = new PositionComponent(position);
            return true;
        }
    }
}
