using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Edges;
using OneMoreSpoon.View.Nodes;

namespace OneMoreSpoon.App.State
{
    public sealed class SelectionVisualService
    {
        private readonly SelectionState selectionState;
        private readonly ViewRegistry viewRegistry;

        public SelectionVisualService(
            SelectionState selectionState,
            ViewRegistry viewRegistry)
        {
            this.selectionState = selectionState;
            this.viewRegistry = viewRegistry;
        }

        public void SelectNode(NodeView nodeView)
        {
            ClearVisualOnly();

            selectionState.SelectNode(nodeView.EntityId);
            nodeView.SetSelected(true);
        }

        public void SelectEdge(EdgeView edgeView)
        {
            ClearVisualOnly();

            selectionState.SelectEdge(edgeView.EntityId);
            edgeView.SetSelected(true);
        }

        public void Clear()
        {
            ClearVisualOnly();
            selectionState.Clear();
        }

        private void ClearVisualOnly()
        {
            if (!selectionState.SelectedEntityId.IsValid)
                return;

            if (!viewRegistry.TryGetView(selectionState.SelectedEntityId, out var view))
                return;

            if (view is ISelectableView selectableView)
                selectableView.SetSelected(false);
        }
    }
}