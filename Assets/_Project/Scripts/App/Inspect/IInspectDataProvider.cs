using OneMoreSpoon.App.State;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.App.Inspect
{
    public interface IInspectDataProvider
    {
        bool CanHandle(SelectionTargetType type);
        InspectPanelData BuildData(GameEntityId entityId);
    }
}
