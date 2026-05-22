namespace OneMoreSpoon.Game.Components
{
    public readonly struct SubstanceComponent
    {
        public string SubstanceId { get; }

        public SubstanceComponent(string substanceId)
        {
            SubstanceId = substanceId;
        }
    }
}
