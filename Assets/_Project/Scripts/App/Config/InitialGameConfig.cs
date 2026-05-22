using OneMoreSpoon.Game.Definitions;

namespace OneMoreSpoon.App.Config
{
    public sealed class InitialGameConfig
    {
        public SO_NodeDefinition[] InitialNodes { get; }

        public InitialGameConfig(SO_NodeDefinition[] initialNodes)
        {
            InitialNodes = initialNodes;
        }
    }
}