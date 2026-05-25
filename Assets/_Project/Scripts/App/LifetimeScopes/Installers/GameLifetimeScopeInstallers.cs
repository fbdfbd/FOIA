using OneMoreSpoon.App.Bootstrap;
using OneMoreSpoon.App.Config;
using OneMoreSpoon.App.Encyclopedia;
using OneMoreSpoon.App.Encyclopedia.Providers;
using OneMoreSpoon.App.Inspect;
using OneMoreSpoon.App.Inspect.Providers;
using OneMoreSpoon.App.Loop;
using OneMoreSpoon.App.Messaging;
using OneMoreSpoon.App.State;
using OneMoreSpoon.App.Tutorial;
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
using OneMoreSpoon.View.UI;
using OneMoreSpoon.View.UI.Encyclopedia;
using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OneMoreSpoon.App.LifetimeScopes.Installers
{
    public readonly struct GameLifetimeScopeRefs
    {
        public GameLifetimeScopeRefs(
            SO_DefinitionCatalog definitionCatalog,
            SO_InitialLevelLayout initialLevelLayout,
            ViewRegistry viewRegistry,
            NodeView inputNodeViewPrefab,
            NodeView outputNodeViewPrefab,
            NodeView interactNodeViewPrefab,
            NodeView mergeNodeViewPrefab,
            EdgeView edgeViewPrefab,
            EdgeBlockIndicatorView edgeBlockIndicatorViewPrefab,
            SubstanceView substanceViewPrefab,
            FlowView flowViewPrefab,
            InspectPanelView inspectPanelViewPrefab,
            EncyclopediaView encyclopediaViewPrefab,
            SpriteRenderer mainGamePanelRenderer,
            float nodePlacementPadding)
        {
            DefinitionCatalog = definitionCatalog;
            InitialLevelLayout = initialLevelLayout;
            ViewRegistry = viewRegistry;
            InputNodeViewPrefab = inputNodeViewPrefab;
            OutputNodeViewPrefab = outputNodeViewPrefab;
            InteractNodeViewPrefab = interactNodeViewPrefab;
            MergeNodeViewPrefab = mergeNodeViewPrefab;
            EdgeViewPrefab = edgeViewPrefab;
            EdgeBlockIndicatorViewPrefab = edgeBlockIndicatorViewPrefab;
            SubstanceViewPrefab = substanceViewPrefab;
            FlowViewPrefab = flowViewPrefab;
            InspectPanelViewPrefab = inspectPanelViewPrefab;
            EncyclopediaViewPrefab = encyclopediaViewPrefab;
            MainGamePanelRenderer = mainGamePanelRenderer;
            NodePlacementPadding = nodePlacementPadding;
        }

        public SO_DefinitionCatalog DefinitionCatalog { get; }
        public SO_InitialLevelLayout InitialLevelLayout { get; }
        public ViewRegistry ViewRegistry { get; }
        public NodeView InputNodeViewPrefab { get; }
        public NodeView OutputNodeViewPrefab { get; }
        public NodeView InteractNodeViewPrefab { get; }
        public NodeView MergeNodeViewPrefab { get; }
        public EdgeView EdgeViewPrefab { get; }
        public EdgeBlockIndicatorView EdgeBlockIndicatorViewPrefab { get; }
        public SubstanceView SubstanceViewPrefab { get; }
        public FlowView FlowViewPrefab { get; }
        public InspectPanelView InspectPanelViewPrefab { get; }
        public EncyclopediaView EncyclopediaViewPrefab { get; }
        public SpriteRenderer MainGamePanelRenderer { get; }
        public float NodePlacementPadding { get; }
    }

    public static class GameLifetimeScopeInstallers
    {
        public static void InstallGameConfig(this IContainerBuilder builder, GameLifetimeScopeRefs refs)
        {
            var definitionCatalog = refs.DefinitionCatalog;
            var initialLevelLayout = refs.InitialLevelLayout;

            builder.RegisterInstance(new InitialGameConfig(
                initialLevelLayout != null
                    ? initialLevelLayout.InitialNodes
                    : Array.Empty<InitialNodeSpawn>()));
            builder.RegisterInstance(new InitialSubstanceInventoryConfig(
                initialLevelLayout != null
                    ? initialLevelLayout.InitialSubstanceStacks
                    : Array.Empty<InitialSubstanceStack>()));

            builder.RegisterInstance(new NodeDefinitionRegistry(
                definitionCatalog != null
                    ? definitionCatalog.NodeDefinitions
                    : Array.Empty<SO_NodeDefinition>()));
            builder.RegisterInstance(new OperationDefinitionRegistry(
                definitionCatalog != null
                    ? definitionCatalog.OperationDefinitions
                    : Array.Empty<SO_OperationDefinition>()));
            builder.RegisterInstance(new SubstanceDefinitionRegistry(
                definitionCatalog != null
                    ? definitionCatalog.SubstanceDefinitions
                    : Array.Empty<SO_SubstanceDefinition>()));
            builder.RegisterInstance(new OutputRuleRegistry(
                definitionCatalog != null
                    ? definitionCatalog.OutputRuleDefinitions
                    : Array.Empty<SO_OutputRuleDefinition>()));
            builder.RegisterInstance(new MergeRecipeRegistry(
                definitionCatalog != null
                    ? definitionCatalog.MergeRecipeDefinitions
                    : Array.Empty<SO_MergeRecipeDefinition>()));
            builder.RegisterInstance(new NodeInspectDefinitionRegistry(
                definitionCatalog != null
                    ? definitionCatalog.NodeInspectDefinitions
                    : Array.Empty<SO_NodeInspectDefinition>()));
            builder.RegisterInstance(new SubstanceInspectDefinitionRegistry(
                definitionCatalog != null
                    ? definitionCatalog.SubstanceInspectDefinitions
                    : Array.Empty<SO_SubstanceInspectDefinition>()));

            builder.RegisterInstance(new PlayAreaBoundsSystem(
                refs.MainGamePanelRenderer,
                refs.NodePlacementPadding));
            builder.RegisterInstance(new NodeViewPrefabSet(
                refs.InputNodeViewPrefab,
                refs.OutputNodeViewPrefab,
                refs.InteractNodeViewPrefab,
                refs.MergeNodeViewPrefab));
        }

        public static void InstallTutorial(this IContainerBuilder builder, TutorialDialogView dialogView, TutorialGoalView goalView)
        {
            builder.RegisterComponent(dialogView);
            builder.RegisterComponent(goalView);
            builder.RegisterEntryPoint<TutorialController>();
        }

        public static void InstallGameCore(this IContainerBuilder builder)
        {
            builder.Register<GameWorld>(Lifetime.Singleton);
            builder.Register<SelectionState>(Lifetime.Singleton);
            builder.Register<EdgeConnectionState>(Lifetime.Singleton);
            builder.Register<ToastMessageQueue>(Lifetime.Singleton);
        }

        public static void InstallGameSystems(this IContainerBuilder builder)
        {
            builder.Register<PlacementRuleSystem>(Lifetime.Singleton);
            builder.Register<NodeMoveSystem>(Lifetime.Singleton);
            builder.Register<ProcessSystem>(Lifetime.Singleton);
            builder.Register<EdgeDeleteSystem>(Lifetime.Singleton);
            builder.Register<SubstanceStackSystem>(Lifetime.Singleton);
            builder.Register<MergeSystem>(Lifetime.Singleton);
            builder.Register<EdgeBlockReturnSystem>(Lifetime.Singleton);
            builder.Register<EdgeBlockEquipSystem>(Lifetime.Singleton);
            builder.Register<ClusterSeparationSystem>(Lifetime.Singleton);
        }

        public static void InstallGameFactories(this IContainerBuilder builder)
        {
            builder.Register<NodeFactory>(Lifetime.Singleton);
            builder.Register<EdgeFactory>(Lifetime.Singleton);
            builder.Register<SubstanceStackFactory>(Lifetime.Singleton);
            builder.Register<NodeViewFactory>(Lifetime.Singleton);
            builder.Register<EdgeViewFactory>(Lifetime.Singleton);
            builder.Register<EdgeBlockIndicatorViewFactory>(Lifetime.Singleton);
            builder.Register<SubstanceViewFactory>(Lifetime.Singleton);
            builder.Register<FlowViewFactory>(Lifetime.Singleton);
        }

        public static void InstallInspect(this IContainerBuilder builder)
        {
            builder.Register<NodeInspectDataProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<SubstanceInspectDataProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<InspectDataService>(Lifetime.Singleton);
            builder.Register<SelectionVisualService>(Lifetime.Singleton);
        }

        public static void InstallEncyclopedia(this IContainerBuilder builder)
        {
            builder.Register<DiscoveryState>(Lifetime.Singleton);
            builder.Register<DiscoveryService>(Lifetime.Singleton);
            builder.Register<RecipeStepTextResolver>(Lifetime.Singleton);
            builder.Register<DishDetailProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<EdgeBlockDetailProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<TraitShardDetailProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<SourceMaterialDetailProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<EncyclopediaDetailService>(Lifetime.Singleton);
        }

        public static void InstallGameViews(this IContainerBuilder builder, GameLifetimeScopeRefs refs)
        {
            builder.RegisterComponent(refs.ViewRegistry);
            builder.RegisterInstance(refs.EdgeViewPrefab);
            builder.RegisterInstance(refs.EdgeBlockIndicatorViewPrefab);
            builder.RegisterInstance(refs.SubstanceViewPrefab);
            builder.RegisterInstance(refs.FlowViewPrefab);
            builder.RegisterInstance(refs.InspectPanelViewPrefab);
            builder.RegisterInstance(refs.EncyclopediaViewPrefab);
        }

        public static void InstallGameInput(this IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<EdgeConnectionInput>();
            builder.RegisterComponentInHierarchy<NodePointerInput>();
            builder.RegisterComponentInHierarchy<EdgeSelectionInput>();
            builder.RegisterComponentInHierarchy<SubstancePointerInput>();
            builder.RegisterComponentInHierarchy<CameraViewportInput>();
            builder.RegisterComponentInHierarchy<MergeSlotDragOutInput>();
            builder.RegisterComponentInHierarchy<TrashCanView>();
        }

        public static void InstallGameEntryPoints(this IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameBootstrap>();
            builder.RegisterEntryPoint<SubstanceInventoryBootstrap>();
            builder.RegisterEntryPoint<GameLoopRunner>();
            builder.RegisterEntryPoint<FlowViewSyncSystem>();
            builder.RegisterEntryPoint<SubstanceViewSyncSystem>();
            builder.RegisterEntryPoint<EdgeBlockIndicatorSyncSystem>();
            builder.RegisterEntryPoint<InspectPanelSyncSystem>();
            builder.RegisterEntryPoint<MergeSlotTextSyncSystem>();
            builder.RegisterEntryPoint<EncyclopediaSyncSystem>();
        }
    }
}
