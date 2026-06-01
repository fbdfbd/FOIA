using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.Game.Systems;
using OneMoreSpoon.Presenter;
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
        private readonly SubstanceTitleProvider titleProvider;
        private readonly SubstanceOutlineColorProvider outlineColorProvider;

        public SubstanceViewFactory(
            SubstanceView prefab,
            GameWorld world,
            ViewRegistry viewRegistry,
            SubstanceDefinitionRegistry definitionRegistry,
            SubstanceDockDepthState dockDepthState,
            SubstanceTitleProvider titleProvider,
            SubstanceOutlineColorProvider outlineColorProvider)
        {
            this.prefab = prefab;
            this.world = world;
            this.viewRegistry = viewRegistry;
            this.definitionRegistry = definitionRegistry;
            this.dockDepthState = dockDepthState;
            this.titleProvider = titleProvider;
            this.outlineColorProvider = outlineColorProvider;
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
            view.Initialize(definitionRegistry, dockDepthState, titleProvider);
            UpdateVisuals(stackId, view);
            viewRegistry.Register(stackId, view);

            return view;
        }

        private void UpdateVisuals(GameEntityId stackId, SubstanceView view)
        {
            if (!world.SubstanceStacks.TryGetValue(stackId, out var stack))
            {
                Debug.LogError($"Substance stack not found: {stackId}");
                return;
            }

            if (!definitionRegistry.TryGet(stack.SubstanceId, out var definition))
            {
                Debug.LogWarning($"Substance definition not found: {stack.SubstanceId}");
                return;
            }

            view.SetImage(definition.Image);
            view.SetOutlineColors(outlineColorProvider.GetColors(definition.Kind));
        }
    }
}
