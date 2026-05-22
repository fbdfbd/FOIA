namespace OneMoreSpoon.Game.Components
{
    public struct EdgeBlockSlotComponent
    {
        public string EquippedSubstanceId;

        public bool HasBlock => !string.IsNullOrEmpty(EquippedSubstanceId);

        public EdgeBlockSlotComponent(string equippedSubstanceId)
        {
            EquippedSubstanceId = equippedSubstanceId;
        }
    }
}
