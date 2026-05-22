using OneMoreSpoon.Game.Core;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.View.Common
{
    public abstract class EntityView : MonoBehaviour
    {
        public GameEntityId EntityId { get; private set; }
        protected GameWorld World { get; private set; }

        public virtual void Bind(GameEntityId entityId, GameWorld world)
        {
            EntityId = entityId;
            World = world;
        }
    }
}