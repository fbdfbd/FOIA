using OneMoreSpoon.App.Inspect;
using OneMoreSpoon.App.State;
using OneMoreSpoon.View.UI;
using VContainer.Unity;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Presenter
{
    public sealed class InspectPanelSyncSystem : ITickable
    {
        private readonly SelectionState selectionState;
        private readonly InspectDataService inspectDataService;
        private readonly InspectPanelView panelView;

        private GameEntityId lastEntityId = GameEntityId.Invalid;
        private SelectionTargetType lastType = SelectionTargetType.None;

        public InspectPanelSyncSystem(
            SelectionState selectionState,
            InspectDataService inspectDataService,
            InspectPanelView panelView)
        {
            this.selectionState = selectionState;
            this.inspectDataService = inspectDataService;
            this.panelView = panelView;
        }

        public void Tick()
        {
            bool selectionUnchanged =
                selectionState.SelectedEntityId == lastEntityId &&
                selectionState.SelectedType == lastType;

            if (selectionUnchanged)
                return;

            lastEntityId = selectionState.SelectedEntityId;
            lastType = selectionState.SelectedType;

            RefreshPanel();
        }

        private void RefreshPanel()
        {
            if (lastType == SelectionTargetType.None)
            {
                panelView.Hide();
                return;
            }

            var data = inspectDataService.GetData(lastEntityId, lastType);

            if (data == null)
            {
                panelView.Hide();
                return;
            }

            panelView.Show(data);
        }
    }
}
