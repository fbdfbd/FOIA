using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Core
{
    public readonly struct FlowSpawnRequest
    {
        public GameEntityId TargetNodeId { get; }
        public string SubstanceId { get; }

        public FlowSpawnRequest(GameEntityId targetNodeId, string substanceId)
        {
            TargetNodeId = targetNodeId;
            SubstanceId = substanceId;
        }
    }
}
