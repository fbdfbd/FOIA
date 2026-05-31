using OneMoreSpoon.Game.Components;
using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.Game.Factories;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class SubstanceGrantService
    {
        private readonly GameWorld world;
        private readonly SubstanceDefinitionRegistry substanceDefinitions;
        private readonly SubstanceStackFactory stackFactory;

        public SubstanceGrantService(
            GameWorld world,
            SubstanceDefinitionRegistry substanceDefinitions,
            SubstanceStackFactory stackFactory)
        {
            this.world = world;
            this.substanceDefinitions = substanceDefinitions;
            this.stackFactory = stackFactory;
        }

        public bool TryGrantSingleAlive(string substanceId, Vector2 position)
        {
            if (string.IsNullOrWhiteSpace(substanceId))
                return false;

            if (HasAliveSubstance(substanceId))
                return false;

            if (!substanceDefinitions.TryGet(substanceId, out var definition))
                return false;

            stackFactory.CreateStack(definition, 1, false, position);
            return true;
        }

        private bool HasAliveSubstance(string substanceId)
        {
            foreach (var stack in world.SubstanceStacks.Values)
            {
                if (stack.SubstanceId == substanceId && (stack.IsInfinite || stack.Amount > 0))
                    return true;
            }

            foreach (var pair in world.Substances)
            {
                if (pair.Value.SubstanceId != substanceId)
                    continue;

                if (!world.Flows.TryGetValue(pair.Key, out var flow))
                    continue;

                if (flow.State != FlowState.Consumed)
                    return true;
            }

            return false;
        }
    }
}
