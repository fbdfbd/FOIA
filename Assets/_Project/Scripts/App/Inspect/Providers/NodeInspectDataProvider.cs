using System;
using OneMoreSpoon.App.State;
using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.App.Inspect.Providers
{
    public sealed class NodeInspectDataProvider : IInspectDataProvider
    {
        private readonly GameWorld world;
        private readonly NodeDefinitionRegistry nodeDefinitions;
        private readonly NodeInspectDefinitionRegistry inspectDefinitions;

        public NodeInspectDataProvider(
            GameWorld world,
            NodeDefinitionRegistry nodeDefinitions,
            NodeInspectDefinitionRegistry inspectDefinitions)
        {
            this.world = world;
            this.nodeDefinitions = nodeDefinitions;
            this.inspectDefinitions = inspectDefinitions;
        }

        public bool CanHandle(SelectionTargetType type) => type == SelectionTargetType.Node;

        public InspectPanelData BuildData(GameEntityId entityId)
        {
            if (!world.Nodes.TryGetValue(entityId, out var node))
                return null;

            nodeDefinitions.TryGet(node.DefinitionId, out var nodeDef);
            inspectDefinitions.TryGet(node.DefinitionId, out var inspectDef);

            var title = nodeDef?.DisplayName ?? node.DefinitionId;
            var description = inspectDef?.Description ?? string.Empty;

            return new InspectPanelData(title, description, Array.Empty<string>());
        }
    }
}
