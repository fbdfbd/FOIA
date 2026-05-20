using System.Collections.Generic;
using FOIA.Core;

namespace FOIA.Runtime
{
    public sealed class ProcessBoardState
    {
        private readonly Dictionary<EdgeId, EdgeBlockId> equippedBlocks = new();
        private readonly Dictionary<EdgeBlockId, int> inventory = new();
        private readonly Dictionary<string, int> accumulatedEffects = new();

        public IReadOnlyDictionary<EdgeId, EdgeBlockId> EquippedBlocks => equippedBlocks;
        public IReadOnlyDictionary<EdgeBlockId, int> Inventory => inventory;
        public IReadOnlyDictionary<string, int> AccumulatedEffects => accumulatedEffects;

        public bool TryGetEquippedBlock(EdgeId edgeId, out EdgeBlockId blockId)
        {
            return equippedBlocks.TryGetValue(edgeId, out blockId);
        }

        public bool EquipBlock(EdgeId edgeId, EdgeBlockId blockId)
        {
            if (!TryConsumeBlock(blockId, 1))
                return false;

            if (equippedBlocks.TryGetValue(edgeId, out var previousBlockId))
                AddBlock(previousBlockId, 1);

            equippedBlocks[edgeId] = blockId;
            return true;
        }

        public bool UnequipBlock(EdgeId edgeId)
        {
            if (!equippedBlocks.Remove(edgeId, out var blockId))
                return false;

            AddBlock(blockId, 1);
            return true;
        }

        public void AddBlock(EdgeBlockId blockId, int amount)
        {
            if (string.IsNullOrWhiteSpace(blockId.Value) || amount <= 0)
                return;

            inventory[blockId] = GetBlockCount(blockId) + amount;
        }

        public bool TryConsumeBlock(EdgeBlockId blockId, int amount)
        {
            if (amount <= 0)
                return true;

            var current = GetBlockCount(blockId);
            if (current < amount)
                return false;

            var next = current - amount;
            if (next == 0)
                inventory.Remove(blockId);
            else
                inventory[blockId] = next;

            return true;
        }

        public int GetBlockCount(EdgeBlockId blockId)
        {
            return inventory.TryGetValue(blockId, out var amount) ? amount : 0;
        }

        public void ApplyEffect(ProcessEffect effect)
        {
            var key = $"{effect.TargetType}:{effect.TargetId}:{effect.EffectType}";
            accumulatedEffects[key] = GetEffectAmount(key) + effect.Amount;
        }

        public int GetEffectAmount(string key)
        {
            return accumulatedEffects.TryGetValue(key, out var amount) ? amount : 0;
        }
    }
}
