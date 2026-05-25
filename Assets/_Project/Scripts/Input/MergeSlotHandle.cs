using UnityEngine;

namespace OneMoreSpoon.Input
{
    public sealed class MergeSlotHandle : MonoBehaviour
    {
        [SerializeField] private int slotIndex;

        public int SlotIndex => slotIndex;
    }
}