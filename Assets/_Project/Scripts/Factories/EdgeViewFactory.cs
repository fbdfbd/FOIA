using OneMoreSpoon.Game.Core;
using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Edges;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.View.Factories
{
    public sealed class EdgeViewFactory
    {
        private readonly EdgeView prefab;
        private readonly GameWorld world;
        private readonly ViewRegistry viewRegistry;

        public EdgeViewFactory(
            EdgeView prefab,
            GameWorld world,
            ViewRegistry viewRegistry)
        {
            this.prefab = prefab;
            this.world = world;
            this.viewRegistry = viewRegistry;
        }

        public EdgeView Create(GameEntityId edgeId)
        {
            var view = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);

            view.Bind(edgeId, world);
            viewRegistry.Register(edgeId, view);

            return view;
        }
    }
}