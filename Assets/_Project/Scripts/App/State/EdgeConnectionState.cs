using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.App.State
{
    public sealed class EdgeConnectionState
    {
        public GameEntityId FromNodeId { get; private set; } = GameEntityId.Invalid;

        public bool IsConnecting => FromNodeId.IsValid;

        public void Begin(GameEntityId fromNodeId)
        {
            FromNodeId = fromNodeId;
        }

        public void Cancel()
        {
            FromNodeId = GameEntityId.Invalid;
        }
    }
}