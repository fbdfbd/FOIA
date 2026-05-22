using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Components
{
    public readonly struct EdgeComponent
    {
        public GameEntityId FromNodeId { get; }
        public GameEntityId ToNodeId { get; }
        public string OperationDefinitionId { get; }

        public EdgeComponent(
            GameEntityId fromNodeId,
            GameEntityId toNodeId,
            string operationDefinitionId)
        {
            FromNodeId = fromNodeId;
            ToNodeId = toNodeId;
            OperationDefinitionId = operationDefinitionId;
        }
    }
}