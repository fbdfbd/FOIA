using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
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
        private readonly SubstanceDefinitionRegistry definitionRegistry;

        public FlowViewFactory(
            FlowView prefab,
            GameWorld world,
            ViewRegistry viewRegistry,
            SubstanceDefinitionRegistry definitionRegistry)
        {
            this.prefab = prefab;
            this.world = world;
            this.viewRegistry = viewRegistry;
            this.definitionRegistry = definitionRegistry;
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
            view.Initialize(definitionRegistry);
            if (world.Flows.TryGetValue(flowEntityId, out var flow))
                view.RenderFlow(flow);

            viewRegistry.Register(flowEntityId, view);

            Debug.Log($"[FlowView] Created entity={flowEntityId}");

            return view;
        }
    }
}
