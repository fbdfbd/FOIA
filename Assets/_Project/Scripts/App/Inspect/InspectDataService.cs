using System.Collections.Generic;
using OneMoreSpoon.App.State;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.App.Inspect
{
    public sealed class InspectDataService
    {
        private readonly IEnumerable<IInspectDataProvider> providers;

        public InspectDataService(IEnumerable<IInspectDataProvider> providers)
        {
            this.providers = providers;
        }

        public InspectPanelData GetData(GameEntityId entityId, SelectionTargetType type)
        {
            foreach (var provider in providers)
                if (provider.CanHandle(type))
                    return provider.BuildData(entityId);

            return null;
        }
    }
}
