using OneMoreSpoon.App.Config;
using OneMoreSpoon.Game.Factories;
using OneMoreSpoon.Game.Systems;
using OneMoreSpoon.View.Factories;
using UnityEngine;
using VContainer.Unity;

namespace OneMoreSpoon.App.Bootstrap
{
    public sealed class GameBootstrap : IStartable
    {
        private readonly InitialGameConfig config;
        private readonly NodeFactory nodeFactory;
        private readonly NodeViewFactory nodeViewFactory;
        private readonly PlayAreaBoundsSystem playAreaBounds;

        public GameBootstrap(
            InitialGameConfig config,
            NodeFactory nodeFactory,
            NodeViewFactory nodeViewFactory,
            PlayAreaBoundsSystem playAreaBounds)
        {
            this.config = config;
            this.nodeFactory = nodeFactory;
            this.nodeViewFactory = nodeViewFactory;
            this.playAreaBounds = playAreaBounds;
        }

        public void Start()
        {
            for (int i = 0; i < config.InitialNodes.Length; i++)
            {
                var definition = config.InitialNodes[i];

                if (definition == null)
                    continue;

                var position = new Vector2(i * 2f, 0f);
                position = playAreaBounds.Clamp(position);

                var entityId = nodeFactory.CreateNode(definition, position);
                nodeViewFactory.Create(entityId);

                Debug.Log($"Created node view: {definition.DisplayName}, {entityId}");
            }
        }
    }
}