using System.Collections.Generic;
using System.Linq;

namespace OneMoreSpoon.Game.Components
{
    public sealed class TagComponent
    {
        private readonly HashSet<string> tags;

        public TagComponent(IEnumerable<string> initialTags)
        {
            tags = new HashSet<string>(initialTags);
        }

        public bool Has(string tag)
        {
            return tags.Contains(tag);
        }

        public bool Add(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return false;

            return tags.Add(tag);
        }

        public void Remove(string tag)
        {
            tags.Remove(tag);
        }

        public string ToDebugString()
        {
            if (tags.Count <= 0)
                return string.Empty;

            return string.Join(", ", tags.OrderBy(tag => tag));
        }
    }
}
