using System;
using System.Collections.Generic;
using UnityEngine;

namespace FOIA.Flow.Runtime
{
    public sealed class NodeFlowStateStore : MonoBehaviour
    {
        private readonly Dictionary<string, List<string>> itemIdsByNodeId = new();
        private readonly Dictionary<string, string> staffIdByNodeId = new();

        public event Action StateChanged;

        public void EquipStaff(string nodeId, string staffId)
        {
            if (string.IsNullOrEmpty(nodeId))
            {
                return;
            }

            staffIdByNodeId[nodeId] = staffId ?? string.Empty;
            StateChanged?.Invoke();
        }

        public string GetStaffId(string nodeId)
        {
            return !string.IsNullOrEmpty(nodeId) && staffIdByNodeId.TryGetValue(nodeId, out string staffId)
                ? staffId
                : string.Empty;
        }

        public void ClearStaff(string nodeId)
        {
            if (!string.IsNullOrEmpty(nodeId) && staffIdByNodeId.Remove(nodeId))
            {
                StateChanged?.Invoke();
            }
        }

        public void AddItem(string nodeId, FlowItem item)
        {
            if (string.IsNullOrEmpty(nodeId) || item == null)
            {
                return;
            }

            RemoveItem(item.ItemId);

            if (!itemIdsByNodeId.TryGetValue(nodeId, out List<string> itemIds))
            {
                itemIds = new List<string>();
                itemIdsByNodeId.Add(nodeId, itemIds);
            }

            itemIds.Add(item.ItemId);
            item.MoveToNode(nodeId);
            item.MoveToContainer(string.Empty);
            StateChanged?.Invoke();
        }

        public bool RemoveItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return false;
            }

            foreach (List<string> itemIds in itemIdsByNodeId.Values)
            {
                if (itemIds.Remove(itemId))
                {
                    StateChanged?.Invoke();
                    return true;
                }
            }

            return false;
        }

        public bool TryGetFirstItemId(string nodeId, out string itemId)
        {
            itemId = string.Empty;

            if (string.IsNullOrEmpty(nodeId) || !itemIdsByNodeId.TryGetValue(nodeId, out List<string> itemIds))
            {
                return false;
            }

            while (itemIds.Count > 0 && string.IsNullOrEmpty(itemIds[0]))
            {
                itemIds.RemoveAt(0);
            }

            if (itemIds.Count == 0)
            {
                return false;
            }

            itemId = itemIds[0];
            return true;
        }

        public int GetItemCount(string nodeId)
        {
            return !string.IsNullOrEmpty(nodeId) && itemIdsByNodeId.TryGetValue(nodeId, out List<string> itemIds)
                ? itemIds.Count
                : 0;
        }

        public IReadOnlyList<string> GetNodesWithItems()
        {
            var result = new List<string>();

            foreach (KeyValuePair<string, List<string>> kvp in itemIdsByNodeId)
            {
                if (kvp.Value.Count > 0)
                {
                    result.Add(kvp.Key);
                }
            }

            return result;
        }
    }
}
