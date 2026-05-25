namespace OneMoreSpoon.App.Config
{
    public sealed class InitialSubstanceInventoryConfig
    {
        public InitialSubstanceStack[] InitialStacks { get; }

        public InitialSubstanceInventoryConfig(InitialSubstanceStack[] initialStacks)
        {
            InitialStacks = initialStacks ?? new InitialSubstanceStack[0];
        }
    }
}
