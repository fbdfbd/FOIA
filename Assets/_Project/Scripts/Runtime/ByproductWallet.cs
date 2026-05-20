using System.Collections.Generic;
using FOIA.Core;

namespace FOIA.Runtime
{
    public sealed class ByproductWallet
    {
        private readonly Dictionary<ByproductType, int> amounts = new();

        public IReadOnlyDictionary<ByproductType, int> Amounts => amounts;

        public int Get(ByproductType type) => amounts.TryGetValue(type, out var amount) ? amount : 0;

        public void Add(ByproductType type, int amount)
        {
            if (type == ByproductType.None || amount <= 0)
                return;

            amounts[type] = Get(type) + amount;
        }

        public bool CanSpend(IReadOnlyList<ByproductAmount> costs)
        {
            foreach (var cost in costs)
            {
                if (Get(cost.type) < cost.amount)
                    return false;
            }

            return true;
        }

        public bool TrySpend(IReadOnlyList<ByproductAmount> costs)
        {
            if (!CanSpend(costs))
                return false;

            foreach (var cost in costs)
            {
                var next = Get(cost.type) - cost.amount;
                if (next == 0)
                    amounts.Remove(cost.type);
                else
                    amounts[cost.type] = next;
            }

            return true;
        }
    }
}
