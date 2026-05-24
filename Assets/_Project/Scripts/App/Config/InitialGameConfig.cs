namespace OneMoreSpoon.App.Config
{
    public sealed class InitialGameConfig
    {
        public InitialNodeSpawn[] InitialNodes { get; }

        public InitialGameConfig(InitialNodeSpawn[] initialNodes)
        {
            InitialNodes = initialNodes ?? new InitialNodeSpawn[0];
        }
    }
}
