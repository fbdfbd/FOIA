using OneMoreSpoon.App.Config;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.Game.Factories;
using OneMoreSpoon.Game.Systems;
using UnityEngine;
using VContainer.Unity;

namespace OneMoreSpoon.App.Bootstrap
{
    public sealed class SubstanceInventoryBootstrap : IStartable
    {
        private readonly InitialSubstanceInventoryConfig config;
        private readonly SubstanceStackFactory stackFactory;
        private readonly PlayAreaBoundsSystem playAreaBounds;

        public SubstanceInventoryBootstrap(
            InitialSubstanceInventoryConfig config,
            SubstanceStackFactory stackFactory, 
            PlayAreaBoundsSystem playAreaBounds)
        {
            this.config = config;
            this.stackFactory = stackFactory;
            this.playAreaBounds = playAreaBounds;
        }

        public void Start()
        {
            for (int i = 0; i < config.InitialStacks.Length; i++)
            {
                var stack = config.InitialStacks[i];

                if (stack == null || stack.SubstanceDefinition == null)
                    continue;

                if (!CanCreateInitialStack(stack.SubstanceDefinition.Kind))
                    continue;

                Vector2 position = stack.Position;
                position = playAreaBounds.Clamp(position); 

                stackFactory.CreateStack(
                    stack.SubstanceDefinition,
                    stack.Amount,
                    stack.IsInfinite,
                    position
                );
            }
        }

        private static bool CanCreateInitialStack(SubstanceKind kind)
        {
            return kind != SubstanceKind.Trash;
        }
    }
}
