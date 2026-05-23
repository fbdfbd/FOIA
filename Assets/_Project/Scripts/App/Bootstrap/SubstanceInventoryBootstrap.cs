using OneMoreSpoon.App.Config;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.Game.Factories;
using UnityEngine;
using VContainer.Unity;

namespace OneMoreSpoon.App.Bootstrap
{
    public sealed class SubstanceInventoryBootstrap : IStartable
    {
        private readonly InitialSubstanceInventoryConfig config;
        private readonly SubstanceStackFactory stackFactory;

        public SubstanceInventoryBootstrap(
            InitialSubstanceInventoryConfig config,
            SubstanceStackFactory stackFactory)
        {
            this.config = config;
            this.stackFactory = stackFactory;
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

                Vector2 position = config.Origin + config.Spacing * i;

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
            return kind == SubstanceKind.Material
                || kind == SubstanceKind.SourceMaterial
                || kind == SubstanceKind.TraitShard
                || kind == SubstanceKind.EdgeBlock;
        }
    }
}
