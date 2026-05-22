using OneMoreSpoon.Game.Definitions;

namespace OneMoreSpoon.Game.Components
{
    public readonly struct NodeComponent
    {
        public string DefinitionId { get; }
        public ProcessLayer ProcessLayer { get; }
        public NodeCategory Category { get; }

        public NodeComponent(
            string definitionId,
            ProcessLayer processLayer,
            NodeCategory category)
        {
            DefinitionId = definitionId;
            ProcessLayer = processLayer;
            Category = category;
        }
    }
}