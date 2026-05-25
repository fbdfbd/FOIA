using System.Collections.Generic;

namespace OneMoreSpoon.App.Encyclopedia
{
    public sealed class DiscoveryState
    {
        private readonly HashSet<string> encountered = new();
        private readonly HashSet<string> unviewedNew = new();

        public bool IsDirty { get; private set; }

        public void MarkEncountered(string substanceId)
        {
            if (encountered.Add(substanceId))
            {
                unviewedNew.Add(substanceId);
                IsDirty = true;
            }
        }

        public void MarkViewed(string substanceId)
        {
            if (unviewedNew.Remove(substanceId))
                IsDirty = true;
        }

        public bool IsEncountered(string substanceId) => encountered.Contains(substanceId);
        public bool IsNew(string substanceId) => unviewedNew.Contains(substanceId);

        public void ClearDirty() => IsDirty = false;
    }
}
