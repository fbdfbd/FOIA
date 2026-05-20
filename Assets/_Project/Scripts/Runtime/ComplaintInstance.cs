using System.Collections.Generic;
using FOIA.Core;

namespace FOIA.Runtime
{
    public sealed class ComplaintInstance
    {
        private readonly Dictionary<ProcessTag, int> tags = new();
        private readonly Dictionary<ByproductType, int> byproducts = new();

        public ComplaintInstance(ComplaintTypeId typeId)
        {
            TypeId = typeId;
        }

        public ComplaintTypeId TypeId { get; }
        public IReadOnlyDictionary<ProcessTag, int> Tags => tags;
        public IReadOnlyDictionary<ByproductType, int> Byproducts => byproducts;

        public int GetTag(ProcessTag tag) => tags.TryGetValue(tag, out var amount) ? amount : 0;
        public int GetByproduct(ByproductType type) => byproducts.TryGetValue(type, out var amount) ? amount : 0;

        public void AddTag(ProcessTag tag, int amount)
        {
            if (tag == ProcessTag.None || amount == 0)
                return;

            tags[tag] = GetTag(tag) + amount;
        }

        public void AddByproduct(ByproductType type, int amount)
        {
            if (type == ByproductType.None || amount <= 0)
                return;

            byproducts[type] = GetByproduct(type) + amount;
        }
    }
}
