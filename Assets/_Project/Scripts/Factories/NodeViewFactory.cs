using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Nodes;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.View.Factories
{
    public sealed class NodeViewFactory
    {
        private readonly NodeView prefab;
        private readonly GameWorld world;
        private readonly NodeDefinitionRegistry nodeDefinitionRegistry;
        private readonly ViewRegistry viewRegistry;

        public NodeViewFactory(
            NodeView prefab,
            GameWorld world,
            NodeDefinitionRegistry nodeDefinitionRegistry,
            ViewRegistry viewRegistry)
        {
            this.prefab = prefab;
            this.world = world;
            this.nodeDefinitionRegistry = nodeDefinitionRegistry;
            this.viewRegistry = viewRegistry;
        }

        public NodeView Create(GameEntityId entityId)
        {
            if (!world.Positions.TryGetValue(entityId, out var position))
            {
                Debug.LogError($"Node position not found: {entityId}");
                return null;
            }

            var view = Object.Instantiate(prefab, position.Value, Quaternion.identity);
            view.Bind(entityId, world);
            UpdateLabel(entityId, view);

            viewRegistry.Register(entityId, view);

            return view;
        }

        private void UpdateLabel(GameEntityId entityId, NodeView view)
        {
            if (!world.Nodes.TryGetValue(entityId, out var node))
            {
                Debug.LogError($"Node not found: {entityId}");
                return;
            }

            if (!nodeDefinitionRegistry.TryGet(node.DefinitionId, out var definition))
            {
                Debug.LogWarning($"Node definition not found: {node.DefinitionId}");
                view.SetLabel(node.DefinitionId);
                return;
            }

            view.SetLabel(definition.DisplayName);
        }
    }
}
