using OneMoreSpoon.Game.Core;
using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Flows;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.View.Factories
{
    public sealed class FlowViewFactory
    {
        private readonly FlowView prefab;
        private readonly GameWorld world;
        private readonly ViewRegistry viewRegistry;

        public FlowViewFactory(
            FlowView prefab,
            GameWorld world,
            ViewRegistry viewRegistry)
        {
            this.prefab = prefab;
            this.world = world;
            this.viewRegistry = viewRegistry;
        }

        public FlowView Create(GameEntityId flowEntityId)
        {
            if (prefab == null)
            {
                Debug.LogError("FlowView prefab is not assigned.");
                return null;
            }

            var view = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);

            view.Bind(flowEntityId, world);
            viewRegistry.Register(flowEntityId, view);

            Debug.Log($"[FlowView] Created entity={flowEntityId}");

            return view;
        }
    }
}
