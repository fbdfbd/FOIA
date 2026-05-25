using System.Collections.Generic;

namespace OneMoreSpoon.Game.Components
{
    public sealed class EdgeBlockSlotComponent
    {
        private readonly List<string> equippedSubstanceIds = new();

        public IReadOnlyList<string> EquippedSubstanceIds => equippedSubstanceIds;
        public bool HasBlock => equippedSubstanceIds.Count > 0;

        public EdgeBlockSlotComponent()
        {
        }

        public EdgeBlockSlotComponent(string equippedSubstanceId)
        {
            Add(equippedSubstanceId);
        }

        public bool Contains(string substanceId)
        {
            return !string.IsNullOrWhiteSpace(substanceId)
                && equippedSubstanceIds.Contains(substanceId);
        }

        public bool Add(string substanceId)
        {
            if (string.IsNullOrWhiteSpace(substanceId) || Contains(substanceId))
                return false;

            equippedSubstanceIds.Add(substanceId);
            return true;
        }

        public void Clear()
        {
            equippedSubstanceIds.Clear();
        }
    }
}
