using OneMoreSpoon.App.LifetimeScopes.Installers;
using OneMoreSpoon.App.Config;
using OneMoreSpoon.App.Tutorial;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.Game.Systems;
using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Edges;
using OneMoreSpoon.View.Flows;
using OneMoreSpoon.View.Nodes;
using OneMoreSpoon.View.Substances;
using OneMoreSpoon.View.UI;
using OneMoreSpoon.View.UI.Encyclopedia;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace OneMoreSpoon.App.LifetimeScopes
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [Header("Initial Test Data")]
        [SerializeField] private SO_DefinitionCatalog definitionCatalog;
        [SerializeField] private SO_InitialLevelLayout initialLevelLayout;

        [Header("View")]
        [SerializeField] private ViewRegistry viewRegistry;
        [SerializeField] private NodeView inputNodeViewPrefab;
        [SerializeField] private NodeView outputNodeViewPrefab;
        [SerializeField] private NodeView interactNodeViewPrefab;
        [SerializeField] private NodeView mergeNodeViewPrefab;
        [SerializeField] private EdgeView edgeViewPrefab;
        [SerializeField] private EdgeBlockIndicatorView edgeBlockIndicatorViewPrefab;
        [SerializeField] private SubstanceView substanceViewPrefab;
        [SerializeField] private SO_SubstanceOutlineConfig substanceOutlineConfig;
        [SerializeField] private FlowView flowViewPrefab;
        [SerializeField] private InspectPanelView inspectPanelViewPrefab;
        [SerializeField] private EncyclopediaView encyclopediaViewPrefab;

        [Header("Substance Docks")]
        [SerializeField] private SubstanceDockAreaView dishDockArea;
        [SerializeField] private SubstanceDockAreaView edgeBlockDockArea;
        [SerializeField] private SubstanceDockAreaView traitShardDockArea;
        [SerializeField] private SubstanceDockLayoutSettings substanceDockLayoutSettings = new();

        [Header("Tutorial")]
        [SerializeField] private bool enableTutorial;
        [SerializeField] private TutorialDialogView tutorialDialogView;
        [SerializeField] private TutorialGoalView tutorialGoalView;

        [Header("Play Area")]
        [SerializeField] private SpriteRenderer mainGamePanelRenderer;
        [FormerlySerializedAs("nodePlaecementPadding")]
        [SerializeField] private float nodePlacementPadding = 0.4f;

        protected override void Configure(IContainerBuilder builder)
        {
            var refs = CreateRefs();

            builder.InstallGameConfig(refs);
            builder.InstallGameCore();
            builder.InstallGameSystems(refs);
            builder.InstallGameFactories();
            builder.InstallInspect();
            builder.InstallEncyclopedia();
            builder.InstallGameViews(refs);
            builder.InstallGameInput();
            builder.InstallGameEntryPoints();

            if (enableTutorial)
                builder.InstallTutorial(tutorialDialogView, tutorialGoalView);
        }

        private GameLifetimeScopeRefs CreateRefs()
        {
            return new GameLifetimeScopeRefs(
                definitionCatalog,
                initialLevelLayout,
                viewRegistry,
                inputNodeViewPrefab,
                outputNodeViewPrefab,
                interactNodeViewPrefab,
                mergeNodeViewPrefab,
                edgeViewPrefab,
                edgeBlockIndicatorViewPrefab,
                substanceViewPrefab,
                substanceOutlineConfig,
                flowViewPrefab,
                inspectPanelViewPrefab,
                encyclopediaViewPrefab,
                dishDockArea,
                edgeBlockDockArea,
                traitShardDockArea,
                substanceDockLayoutSettings,
                mainGamePanelRenderer,
                nodePlacementPadding);
        }
    }
}
