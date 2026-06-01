using System.Collections.Generic;

namespace OneMoreSpoon.Game.Core
{
    public sealed class GameWorldChanges
    {
        public readonly HashSet<EntityId> PositionChanged = new();
        public readonly HashSet<EntityId> SubstanceStackChanged = new();
        public readonly HashSet<EntityId> EdgeChanged = new();
        public readonly HashSet<EntityId> EdgeBlockChanged = new();

        public void MarkPositionChanged(EntityId entityId)
        {
            if (entityId.IsValid)
                PositionChanged.Add(entityId);
        }

        public void MarkSubstanceStackChanged(EntityId stackId)
        {
            if (stackId.IsValid)
                SubstanceStackChanged.Add(stackId);
        }

        public void MarkEdgeChanged(EntityId edgeId)
        {
            if (edgeId.IsValid)
                EdgeChanged.Add(edgeId);
        }

        public void MarkEdgeBlockChanged(EntityId edgeId)
        {
            if (edgeId.IsValid)
                EdgeBlockChanged.Add(edgeId);
        }

        public void Clear()
        {
            PositionChanged.Clear();
            SubstanceStackChanged.Clear();
            EdgeChanged.Clear();
            EdgeBlockChanged.Clear();
        }
    }
}
