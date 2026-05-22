using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.App.State
{
    public sealed class SelectionState
    {
        public GameEntityId SelectedEntityId { get; private set; } = GameEntityId.Invalid;

        public void Select(GameEntityId entityId)
        {
            SelectedEntityId = entityId;
        }

        public void Clear()
        {
            SelectedEntityId = GameEntityId.Invalid;
        }
    }
}