namespace OneMoreSpoon.Game.Components
{
    public readonly struct NodeComponent
    {
        public string DefinitionId { get; }

        public NodeComponent(string definitionId)
        {
            DefinitionId = definitionId;
        }
    }
}