namespace OneMoreSpoon.App.Encyclopedia.Data
{
    public sealed class EncyclopediaTabBadgeData
    {
        public EncyclopediaTab Tab { get; }
        public bool HasNewEntry { get; }

        public EncyclopediaTabBadgeData(EncyclopediaTab tab, bool hasNewEntry)
        {
            Tab = tab;
            HasNewEntry = hasNewEntry;
        }
    }
}
