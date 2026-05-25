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
                var spawn = config.InitialNodes[i];

                if (spawn == null || spawn.Definition == null)
                    continue;

                var definition = spawn.Definition;
                var position = spawn.Position;
                position = playAreaBounds.Clamp(position);

                var entityId = nodeFactory.CreateNode(definition, position);
                nodeViewFactory.Create(entityId);

                Debug.Log($"Created node view: {definition.DisplayName}, {entityId}");
            }
        }
    }
}
