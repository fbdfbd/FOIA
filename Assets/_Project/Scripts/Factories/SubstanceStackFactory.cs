using OneMoreSpoon.App.Encyclopedia;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.Game.Systems;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Factories
{
    public sealed class SubstanceStackFactory
    {
        private readonly DiscoveryService discoveryService;
        private readonly SubstanceStackSpawnService stackSpawnService;

        public SubstanceStackFactory(
            DiscoveryService discoveryService,
            SubstanceStackSpawnService stackSpawnService)
        {
            this.discoveryService = discoveryService;
            this.stackSpawnService = stackSpawnService;
        }

        public GameEntityId CreateStack(
            SO_SubstanceDefinition definition,
            int amount,
            bool isInfinite,
            Vector2 position)
        {
            discoveryService.NotifyEncountered(definition.SubstanceId);

            return stackSpawnService.CreateOrTransitionStack(
                definition,
                Mathf.Max(0, amount),
                isInfinite,
                position);
        }
    }
}
