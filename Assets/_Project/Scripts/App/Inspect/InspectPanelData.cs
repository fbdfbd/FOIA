using System.Collections.Generic;

namespace OneMoreSpoon.App.Inspect
{
    public sealed class InspectPanelData
    {
        public string Title { get; }
        public string Description { get; }
        public IReadOnlyList<string> Tags { get; }

        public InspectPanelData(
            string title,
            string description,
            IReadOnlyList<string> tags)
        {
            Title = title;
            Description = description;
            Tags = tags;
        }
    }
}
