using FOIA.Presentation;
using FOIA.Runtime;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace FOIA.Infrastructure
{
    public sealed class PocLifetimeScope : LifetimeScope
    {
        [SerializeField] private DefinitionCatalog catalog;
        [SerializeField] private EdgeBlockDragController edgeBlockDragController;
        [SerializeField] private ProcessRunController processRunController;
        [SerializeField] private CraftingController craftingController;
        [SerializeField] private PocStateBootstrapper stateBootstrapper;
        [SerializeField] private NodeConnectionInputController nodeConnectionInputController;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(catalog);
            builder.Register<ProcessBoardState>(Lifetime.Singleton);
            builder.Register<ByproductWallet>(Lifetime.Singleton);
            builder.Register<ProcessSimulator>(Lifetime.Singleton);
            builder.Register<CraftingService>(Lifetime.Singleton);

            RegisterSceneComponent(builder, edgeBlockDragController);
            RegisterSceneComponent(builder, processRunController);
            RegisterSceneComponent(builder, craftingController);
            RegisterSceneComponent(builder, stateBootstrapper);
            RegisterSceneComponent(builder, nodeConnectionInputController);
        }

        private static void RegisterSceneComponent<T>(IContainerBuilder builder, T component)
            where T : Component
        {
            if (component != null)
                builder.RegisterComponent(component);
        }
    }
}
