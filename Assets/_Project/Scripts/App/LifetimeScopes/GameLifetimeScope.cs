using OneMoreSpoon.App.Bootstrap;
using OneMoreSpoon.App.Config;
using OneMoreSpoon.App.Loop;
using OneMoreSpoon.App.State;
using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.Game.Factories;
using OneMoreSpoon.Game.Systems;
using OneMoreSpoon.Input;
using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Edges;
using OneMoreSpoon.View.Factories;
using OneMoreSpoon.View.Nodes;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OneMoreSpoon.App.LifetimeScopes
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [Header("Initial Test Data")]
        [SerializeField] private SO_NodeDefinition[] initialNodeDefinitions;

        [Header("View")]
        [SerializeField] private ViewRegistry viewRegistry;
        [SerializeField] private NodeView nodeViewPrefab;
        [SerializeField] private EdgeView edgeViewPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            // ── Config ─────────────────────────────────────────────
            builder.RegisterInstance(new InitialGameConfig(initialNodeDefinitions));

            // ── Core State ─────────────────────────────────────────
            builder.Register<GameWorld>(Lifetime.Singleton);
            builder.Register<SelectionState>(Lifetime.Singleton);
            builder.Register<EdgeConnectionState>(Lifetime.Singleton);

            // ── Systems ────────────────────────────────────────────
            builder.Register<PlacementRuleSystem>(Lifetime.Singleton);
            builder.Register<NodeMoveSystem>(Lifetime.Singleton);
            builder.Register<ProcessSystem>(Lifetime.Singleton);

            // ── Factories ──────────────────────────────────────────
            builder.Register<NodeFactory>(Lifetime.Singleton);
            builder.Register<EdgeFactory>(Lifetime.Singleton);
            builder.Register<NodeViewFactory>(Lifetime.Singleton);
            builder.Register<EdgeViewFactory>(Lifetime.Singleton);

            // ── View (Prefabs & Registry) ──────────────────────────
            builder.RegisterComponent(viewRegistry);
            builder.RegisterInstance(nodeViewPrefab);
            builder.RegisterInstance(edgeViewPrefab);

            // ── Input ──────────────────────────────────────────────
            builder.RegisterComponentInHierarchy<EdgeConnectionInput>();
            builder.RegisterComponentInHierarchy<NodePointerInput>();

            // ── Entry Points ───────────────────────────────────────
            builder.RegisterEntryPoint<GameBootstrap>();
            builder.RegisterEntryPoint<GameLoopRunner>();
        }
    }
}