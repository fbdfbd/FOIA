using System.Collections.Generic;

namespace OneMoreSpoon.Game.Components
{
    public sealed class FlowHistoryComponent
    {
        private readonly List<string> entries = new();

        public IReadOnlyList<string> Entries => entries;

        public void Add(string entry)
        {
            if (!string.IsNullOrWhiteSpace(entry))
                entries.Add(entry);
        }
    }
}
