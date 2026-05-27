using System.Collections.Generic;

namespace OneMoreSpoon.App.Rewards
{
    public sealed class NodeInventoryState
    {
        private readonly Dictionary<string, int> counts = new();

        public void Add(string nodeDefinitionId, int amount)
        {
            if (string.IsNullOrWhiteSpace(nodeDefinitionId) || amount <= 0)
                return;

            counts.TryGetValue(nodeDefinitionId, out var current);
            counts[nodeDefinitionId] = current + amount;
        }

        public int GetCount(string nodeDefinitionId)
        {
            return counts.TryGetValue(nodeDefinitionId, out var count)
                ? count
                : 0;
        }

        public bool TryConsume(string nodeDefinitionId)
        {
            if (!counts.TryGetValue(nodeDefinitionId, out var count) || count <= 0)
                return false;

            if (count == 1)
                counts.Remove(nodeDefinitionId);
            else
                counts[nodeDefinitionId] = count - 1;

            return true;
        }
    }
}
