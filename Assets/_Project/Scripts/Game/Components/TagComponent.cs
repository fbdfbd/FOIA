using System.Collections.Generic;

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

        public void Add(string tag)
        {
            tags.Add(tag);
        }

        public void Remove(string tag)
        {
            tags.Remove(tag);
        }
    }
}