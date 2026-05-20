using System;
using FOIA.Flow.Definitions;
using UnityEngine;

namespace FOIA.Flow.Runtime
{
    public sealed class ProcessStateStore : MonoBehaviour
    {
        public event Action StateChanged;

        public string EquippedStaffId { get; private set; }
        public FlowItem CurrentDocument { get; private set; }
        public FlowItem EquippedEdgeBlockItem { get; private set; }
        public FlowItem FirstCraftItem { get; private set; }
        public FlowItem SecondCraftItem { get; private set; }

        public EdgeBlockDefinition EquippedEdgeBlock =>
            EquippedEdgeBlockItem != null && EquippedEdgeBlockItem.Definition != null
                ? EquippedEdgeBlockItem.Definition.EdgeBlock
                : null;

        public void EquipStaff(string staffId)
        {
            EquippedStaffId = staffId ?? string.Empty;
            StateChanged?.Invoke();
        }

        public void ClearStaff()
        {
            EquippedStaffId = string.Empty;
            StateChanged?.Invoke();
        }

        public void SetCurrentDocument(FlowItem item)
        {
            CurrentDocument = item;
            StateChanged?.Invoke();
        }

        public void ClearCurrentDocument()
        {
            CurrentDocument = null;
            StateChanged?.Invoke();
        }

        public void EquipEdgeBlock(FlowItem item)
        {
            EquippedEdgeBlockItem = item;
            StateChanged?.Invoke();
        }

        public FlowItem UnequipEdgeBlock()
        {
            FlowItem item = EquippedEdgeBlockItem;
            EquippedEdgeBlockItem = null;
            StateChanged?.Invoke();
            return item;
        }

        public void SetCraftItem(int slotIndex, FlowItem item)
        {
            if (slotIndex == 0)
            {
                FirstCraftItem = item;
            }
            else
            {
                SecondCraftItem = item;
            }

            StateChanged?.Invoke();
        }

        public void ClearCraft()
        {
            FirstCraftItem = null;
            SecondCraftItem = null;
            StateChanged?.Invoke();
        }
    }
}
