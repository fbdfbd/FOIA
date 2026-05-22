using OneMoreSpoon.Game.Core;
using System.Collections.Generic;

namespace OneMoreSpoon.Game.Components
{
    public sealed class MergeSlotComponent
    {
        public readonly List<EntityId> StackIds = new();
        public float TimeUntilResolve;
        public bool IsDirty;

        public bool HasEnoughInputs => StackIds.Count >= 2;

        public void AddStack(EntityId stackId, float delay)
        {
            if (!StackIds.Contains(stackId))
                StackIds.Add(stackId);

            TimeUntilResolve = delay;
            IsDirty = true;
        }

        public void MarkResolved()
        {
            TimeUntilResolve = 0f;
            IsDirty = false;
        }

        public void Clear()
        {
            StackIds.Clear();
            MarkResolved();
        }
    }
}
