using System.Collections.Generic;

namespace OneMoreSpoon.Game.Components
{
    public sealed class MergeSlotComponent
    {
        private const int SlotCount = 2;
        private readonly string[] substanceIds = new string[SlotCount];

        public float TimeUntilResolve;
        public bool IsDirty;

        public bool HasLeft => !string.IsNullOrEmpty(substanceIds[0]);
        public bool HasRight => !string.IsNullOrEmpty(substanceIds[1]);
        public bool HasEnoughInputs => HasLeft && HasRight;

        public string GetSubstanceId(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotCount)
                return string.Empty;

            return substanceIds[slotIndex];
        }

        public bool TryAddSubstance(
            string substanceId,
            float delay,
            out int slotIndex)
        {
            slotIndex = -1;

            if (string.IsNullOrWhiteSpace(substanceId))
                return false;

            if (!HasLeft)
            {
                substanceIds[0] = substanceId;
                slotIndex = 0;
                MarkWaiting();
                return true;
            }

            if (!HasRight)
            {
                substanceIds[1] = substanceId;
                slotIndex = 1;
                TimeUntilResolve = delay;
                IsDirty = true;
                return true;
            }

            return false;
        }

        public bool TryRemoveLeft(out string substanceId)
        {
            substanceId = string.Empty;

            if (!HasLeft)
                return false;

            if (HasRight)
                return false;

            substanceId = substanceIds[0];
            substanceIds[0] = string.Empty;
            MarkWaiting();
            return true;
        }

        public void RemoveAt(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotCount)
                return;

            substanceIds[slotIndex] = string.Empty;

            if (!HasEnoughInputs)
                MarkWaiting();
        }

        public void CopySubstanceIds(List<string> buffer)
        {
            buffer.Clear();

            if (HasLeft)
                buffer.Add(substanceIds[0]);

            if (HasRight)
                buffer.Add(substanceIds[1]);
        }

        public void Clear()
        {
            substanceIds[0] = string.Empty;
            substanceIds[1] = string.Empty;
            MarkWaiting();
        }

        public void MarkResolved()
        {
            TimeUntilResolve = 0f;
            IsDirty = false;
        }

        private void MarkWaiting()
        {
            TimeUntilResolve = 0f;
            IsDirty = false;
        }
    }
}