using OneMoreSpoon.App.Encyclopedia;
using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Factories
{
    public sealed class SubstanceStackFactory
    {
        private readonly GameWorld world;
        private readonly DiscoveryService discoveryService;

        public SubstanceStackFactory(GameWorld world, DiscoveryService discoveryService)
        {
            this.world = world;
            this.discoveryService = discoveryService;
        }

        public GameEntityId CreateStack(
            SO_SubstanceDefinition definition,
            int amount,
            bool isInfinite,
            Vector2 position)
        {
            discoveryService.NotifyEncountered(definition.SubstanceId);

            return world.CreateSubstanceStack(
                definition.SubstanceId,
                Mathf.Max(0, amount),
                isInfinite,
                position
            );
        }
    }
}
