namespace OneMoreSpoon.Game.Components
{
    public struct SubstanceStackComponent
    {
        public string SubstanceId;
        public int Amount;
        public bool IsInfinite;

        public bool IsEmpty => !IsInfinite && Amount <= 0;

        public SubstanceStackComponent(string substanceId, int amount, bool isInfinite)
        {
            SubstanceId = substanceId;
            Amount = amount;
            IsInfinite = isInfinite;
        }
    }
}
