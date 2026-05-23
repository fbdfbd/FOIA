using System;
using OneMoreSpoon.App.Bootstrap;
using OneMoreSpoon.App.Config;
using OneMoreSpoon.App.Loop;
using OneMoreSpoon.App.State;
using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.Game.Factories;
using OneMoreSpoon.Game.Systems;
using OneMoreSpoon.Input;
using OneMoreSpoon.Presenter;
using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Edges;
using OneMoreSpoon.View.Factories;
using OneMoreSpoon.View.Flows;
using OneMoreSpoon.View.Nodes;
using OneMoreSpoon.View.Substances;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OneMoreSpoon.App.LifetimeScopes
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [Header("Initial Test Data")]
        [SerializeField] private SO_DefinitionCatalog definitionCatalog;
        [SerializeField] private SO_NodeDefinition[] initialNodeDefinitions;
        [SerializeField] private InitialSubstanceStack[] initialSubstanceStacks;
        [SerializeField] private Vector2 substanceInventoryOrigin = new(-4f, -3f);
        [SerializeField] private Vector2 substanceInventorySpacing = new(1.7f, 0f);

        [Header("View")]
        [SerializeField] private ViewRegistry viewRegistry;
        [SerializeField] private NodeView nodeViewPrefab;
        [SerializeField] private EdgeView edgeViewPrefab;
        [SerializeField] private EdgeBlockIndicatorView edgeBlockIndicatorViewPrefab;
        [SerializeField] private SubstanceView substanceViewPrefab;
        [SerializeField] private FlowView flowViewPrefab;

        [Header("Play Area")]
        [SerializeField] private SpriteRenderer mainGamePanelRenderer;
        [SerializeField] private float nodePlaecementPadding = 0.4f;

        protected override void Configure(IContainerBuilder builder)
        {
            var nodeDefinitions = definitionCatalog != null
                ? definitionCatalog.NodeDefinitions
                : Array.Empty<SO_NodeDefinition>();
            var operationDefinitions = definitionCatalog != null
                ? definitionCatalog.OperationDefinitions
                : Array.Empty<SO_OperationDefinition>();
            var substanceDefinitions = definitionCatalog != null
                ? definitionCatalog.SubstanceDefinitions
                : Array.Empty<SO_SubstanceDefinition>();
            var outputRuleDefinitions = definitionCatalog != null
                ? definitionCatalog.OutputRuleDefinitions
                : Array.Empty<SO_OutputRuleDefinition>();
            var mergeRecipeDefinitions = definitionCatalog != null
                ? definitionCatalog.MergeRecipeDefinitions
                : Array.Empty<SO_MergeRecipeDefinition>();

            // ── Config ─────────────────────────────────────────────
            builder.RegisterInstance(new InitialGameConfig(initialNodeDefinitions));
            builder.RegisterInstance(new InitialSubstanceInventoryConfig(
                initialSubstanceStacks,
                substanceInventoryOrigin,
                substanceInventorySpacing
            ));
            builder.RegisterInstance(new NodeDefinitionRegistry(nodeDefinitions));
            builder.RegisterInstance(new OperationDefinitionRegistry(operationDefinitions));
            builder.RegisterInstance(new SubstanceDefinitionRegistry(substanceDefinitions));
            builder.RegisterInstance(new OutputRuleRegistry(outputRuleDefinitions));
            builder.RegisterInstance(new MergeRecipeRegistry(mergeRecipeDefinitions));
            builder.RegisterInstance(new PlayAreaBoundsSystem(mainGamePanelRenderer, nodePlaecementPadding));

            // ── Core State ─────────────────────────────────────────
            builder.Register<GameWorld>(Lifetime.Singleton);
            builder.Register<SelectionState>(Lifetime.Singleton);
            builder.Register<EdgeConnectionState>(Lifetime.Singleton);

            // ── Systems ────────────────────────────────────────────
            builder.Register<PlacementRuleSystem>(Lifetime.Singleton);
            builder.Register<NodeMoveSystem>(Lifetime.Singleton);
            builder.Register<ProcessSystem>(Lifetime.Singleton);
            builder.Register<EdgeDeleteSystem>(Lifetime.Singleton);
            builder.Register<SubstanceStackSystem>(Lifetime.Singleton);
            builder.Register<MergeSystem>(Lifetime.Singleton);
            builder.Register<EdgeBlockEquipSystem>(Lifetime.Singleton);

            // ── Factories ──────────────────────────────────────────
            builder.Register<NodeFactory>(Lifetime.Singleton);
            builder.Register<EdgeFactory>(Lifetime.Singleton);
            builder.Register<SubstanceStackFactory>(Lifetime.Singleton);
            builder.Register<NodeViewFactory>(Lifetime.Singleton);
            builder.Register<EdgeViewFactory>(Lifetime.Singleton);
            builder.Register<EdgeBlockIndicatorViewFactory>(Lifetime.Singleton);
            builder.Register<SubstanceViewFactory>(Lifetime.Singleton);
            builder.Register<FlowViewFactory>(Lifetime.Singleton);

            // ── Services ──────────────────────────────────────────
            builder.Register<SelectionVisualService>(Lifetime.Singleton);

            // ── View (Prefabs & Registry) ──────────────────────────
            builder.RegisterComponent(viewRegistry);
            builder.RegisterInstance(nodeViewPrefab);
            builder.RegisterInstance(edgeViewPrefab);
            builder.RegisterInstance(edgeBlockIndicatorViewPrefab);
            builder.RegisterInstance(substanceViewPrefab);
            builder.RegisterInstance(flowViewPrefab);

            // ── Input ──────────────────────────────────────────────
            builder.RegisterComponentInHierarchy<EdgeConnectionInput>();
            builder.RegisterComponentInHierarchy<NodePointerInput>();
            builder.RegisterComponentInHierarchy<EdgeSelectionInput>();
            builder.RegisterComponentInHierarchy<SubstancePointerInput>();
            builder.RegisterComponentInHierarchy<CameraViewportInput>();

            // ── Entry Points ───────────────────────────────────────
            builder.RegisterEntryPoint<GameBootstrap>();
            builder.RegisterEntryPoint<SubstanceInventoryBootstrap>();
            builder.RegisterEntryPoint<GameLoopRunner>();
            builder.RegisterEntryPoint<FlowViewSyncSystem>();
            builder.RegisterEntryPoint<SubstanceViewSyncSystem>();
            builder.RegisterEntryPoint<EdgeBlockIndicatorSyncSystem>();
        }
    }
}
