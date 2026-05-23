using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.App.State
{
    public enum SelectionTargetType
    {
        None,
        Node,
        Edge,
        Substance
    }

    public sealed class SelectionState
    {
        public GameEntityId SelectedEntityId { get; private set; } = GameEntityId.Invalid;
        public SelectionTargetType SelectedType { get; private set; } = SelectionTargetType.None;

        public void SelectNode(GameEntityId entityId)
        {
            SelectedEntityId = entityId;
            SelectedType = SelectionTargetType.Node;
        }

        public void SelectEdge(GameEntityId entityId)
        {
            SelectedEntityId = entityId;
            SelectedType = SelectionTargetType.Edge;
        }

        public void SelectSubstance(GameEntityId entityId)
        {
            SelectedEntityId = entityId;
            SelectedType = SelectionTargetType.Substance;
        }

        public void Clear()
        {
            SelectedEntityId = GameEntityId.Invalid;
            SelectedType = SelectionTargetType.None;
        }
    }
}