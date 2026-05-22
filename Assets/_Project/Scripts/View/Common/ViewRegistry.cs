using System.Collections.Generic;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.View.Common
{
    public sealed class ViewRegistry : MonoBehaviour
    {
        private readonly Dictionary<GameEntityId, EntityView> views = new();

        public void Register(GameEntityId entityId, EntityView view)
        {
            views[entityId] = view;
        }

        public void Unregister(GameEntityId entityId)
        {
            views.Remove(entityId);
        }

        public bool TryGetView(GameEntityId entityId, out EntityView view)
        {
            return views.TryGetValue(entityId, out view);
        }
    }
}