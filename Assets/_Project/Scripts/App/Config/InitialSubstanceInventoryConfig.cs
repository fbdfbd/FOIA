using UnityEngine;

namespace OneMoreSpoon.App.Config
{
    public sealed class InitialSubstanceInventoryConfig
    {
        public InitialSubstanceStack[] InitialStacks { get; }
        public Vector2 Origin { get; }
        public Vector2 Spacing { get; }

        public InitialSubstanceInventoryConfig(
            InitialSubstanceStack[] initialStacks,
            Vector2 origin,
            Vector2 spacing)
        {
            InitialStacks = initialStacks ?? new InitialSubstanceStack[0];
            Origin = origin;
            Spacing = spacing;
        }
    }
}
