namespace OneMoreSpoon.App.Encyclopedia.Data
{
    public sealed class EncyclopediaEntryData
    {
        public string SubstanceId { get; }
        public string DisplayName { get; }
        public bool IsEncountered { get; }
        public bool IsNew { get; }

        public EncyclopediaEntryData(string substanceId, string displayName, bool isEncountered, bool isNew)
        {
            SubstanceId = substanceId;
            DisplayName = displayName;
            IsEncountered = isEncountered;
            IsNew = isNew;
        }
    }
}
