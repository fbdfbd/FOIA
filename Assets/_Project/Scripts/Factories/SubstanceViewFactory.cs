using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.Game.Systems;
using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Substances;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.View.Factories
{
    public sealed class SubstanceViewFactory
    {
        private readonly SubstanceView prefab;
        private readonly GameWorld world;
        private readonly ViewRegistry viewRegistry;
        private readonly SubstanceDefinitionRegistry definitionRegistry;
        private readonly SubstanceDockDepthState dockDepthState;

        public SubstanceViewFactory(
            SubstanceView prefab,
            GameWorld world,
            ViewRegistry viewRegistry,
            SubstanceDefinitionRegistry definitionRegistry,
            SubstanceDockDepthState dockDepthState)
        {
            this.prefab = prefab;
            this.world = world;
            this.viewRegistry = viewRegistry;
            this.definitionRegistry = definitionRegistry;
            this.dockDepthState = dockDepthState;
        }

        public SubstanceView Create(GameEntityId stackId)
        {
            if (!world.Positions.TryGetValue(stackId, out var position))
            {
                Debug.LogError($"Substance stack position not found: {stackId}");
                return null;
            }

            var view = Object.Instantiate(prefab, position.Value, Quaternion.identity);

            view.Bind(stackId, world);
            view.Initialize(definitionRegistry, dockDepthState);
            viewRegistry.Register(stackId, view);

            return view;
        }
    }
}
