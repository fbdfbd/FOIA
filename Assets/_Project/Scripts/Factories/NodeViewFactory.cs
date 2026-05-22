using OneMoreSpoon.Game.Core;
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
        private readonly ViewRegistry viewRegistry;

        public NodeViewFactory(
            NodeView prefab,
            GameWorld world,
            ViewRegistry viewRegistry)
        {
            this.prefab = prefab;
            this.world = world;
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

            viewRegistry.Register(entityId, view);

            return view;
        }
    }
}