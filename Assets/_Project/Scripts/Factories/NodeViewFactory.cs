using OneMoreSpoon.Game.Components;
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
        private readonly NodeViewPrefabSet prefabSet;
        private readonly GameWorld world;
        private readonly NodeDefinitionRegistry nodeDefinitionRegistry;
        private readonly ViewRegistry viewRegistry;

        public NodeViewFactory(
            NodeViewPrefabSet prefabSet,
            GameWorld world,
            NodeDefinitionRegistry nodeDefinitionRegistry,
            ViewRegistry viewRegistry)
        {
            this.prefabSet = prefabSet;
            this.world = world;
            this.nodeDefinitionRegistry = nodeDefinitionRegistry;
            this.viewRegistry = viewRegistry;
        }

        public NodeView Create(GameEntityId entityId)
        {
            if (!world.Positions.TryGetValue(entityId, out PositionComponent position))
            {
                Debug.LogError($"Node position not found: {entityId}");
                return null;
            }

            if (!world.Nodes.TryGetValue(entityId, out NodeComponent node))
            {
                Debug.LogError($"Node not found: {entityId}");
                return null;
            }

            NodeView prefab = GetPrefab(node.Category);

            if (prefab == null)
            {
                Debug.LogError($"Node prefab not found for category: {node.Category}");
                return null;
            }

            NodeView view = Object.Instantiate(prefab, position.Value, Quaternion.identity);

            view.Bind(entityId, world);
            UpdateVisuals(entityId, view);

            viewRegistry.Register(entityId, view);

            return view;
        }

        private void UpdateVisuals(GameEntityId entityId, NodeView view)
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
            view.SetImage(definition.Image);
        }
        private NodeView GetPrefab(NodeCategory category)
        {
            switch (category)
            {
                case NodeCategory.Input:
                    return prefabSet.Input;

                case NodeCategory.Output:
                    return prefabSet.Output;

                case NodeCategory.Merge:
                    return prefabSet.Merge;

                //case NodeCategory.Process:
                //case NodeCategory.Split:
                //case NodeCategory.Storage:
                    //return prefabSet.Interact;

                default:
                    return prefabSet.Interact;
            }
        }
    }
}
