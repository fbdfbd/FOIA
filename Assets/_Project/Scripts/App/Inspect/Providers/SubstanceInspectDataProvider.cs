using System;
using OneMoreSpoon.App.State;
using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.App.Inspect.Providers
{
    public sealed class SubstanceInspectDataProvider : IInspectDataProvider
    {
        private readonly GameWorld world;
        private readonly SubstanceDefinitionRegistry substanceDefinitions;
        private readonly SubstanceInspectDefinitionRegistry inspectDefinitions;

        public SubstanceInspectDataProvider(
            GameWorld world,
            SubstanceDefinitionRegistry substanceDefinitions,
            SubstanceInspectDefinitionRegistry inspectDefinitions)
        {
            this.world = world;
            this.substanceDefinitions = substanceDefinitions;
            this.inspectDefinitions = inspectDefinitions;
        }

        public bool CanHandle(SelectionTargetType type) => type == SelectionTargetType.Substance;

        public InspectPanelData BuildData(GameEntityId entityId)
        {
            if (!world.SubstanceStacks.TryGetValue(entityId, out var stack))
                return null;

            substanceDefinitions.TryGet(stack.SubstanceId, out var substanceDef);
            inspectDefinitions.TryGet(stack.SubstanceId, out var inspectDef);

            var title = substanceDef?.DisplayName ?? stack.SubstanceId;
            var description = inspectDef?.Description ?? string.Empty;

            return new InspectPanelData(title, description, Array.Empty<string>());
        }
    }
}
