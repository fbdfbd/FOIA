using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Factories
{
    public sealed class SubstanceStackFactory
    {
        private readonly GameWorld world;

        public SubstanceStackFactory(GameWorld world)
        {
            this.world = world;
        }

        public GameEntityId CreateStack(
            SO_SubstanceDefinition definition,
            int amount,
            bool isInfinite,
            Vector2 position)
        {
            return world.CreateSubstanceStack(
                definition.SubstanceId,
                Mathf.Max(0, amount),
                isInfinite,
                position
            );
        }
    }
}
