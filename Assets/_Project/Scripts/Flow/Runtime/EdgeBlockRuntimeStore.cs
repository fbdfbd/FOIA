using System;
using System.Collections.Generic;
using FOIA.Flow.Definitions;
using UnityEngine;

namespace FOIA.Flow.Runtime
{
    public sealed class EdgeBlockRuntimeStore : MonoBehaviour
    {
        private readonly Dictionary<string, FlowItem> blockItemsByEdgeId = new();

        public event Action BlocksChanged;

        public void Equip(string edgeId, FlowItem blockItem)
        {
            if (string.IsNullOrEmpty(edgeId) || blockItem == null || blockItem.Definition.Kind != FlowItemKind.EdgeBlock)
            {
                return;
            }

            blockItemsByEdgeId[edgeId] = blockItem;
            BlocksChanged?.Invoke();
        }

        public FlowItem Unequip(string edgeId)
        {
            if (string.IsNullOrEmpty(edgeId) || !blockItemsByEdgeId.TryGetValue(edgeId, out FlowItem blockItem))
            {
                return null;
            }

            blockItemsByEdgeId.Remove(edgeId);
            BlocksChanged?.Invoke();
            return blockItem;
        }

        public EdgeBlockDefinition GetBlock(string edgeId)
        {
            return blockItemsByEdgeId.TryGetValue(edgeId, out FlowItem blockItem)
                ? blockItem.Definition.EdgeBlock
                : null;
        }

        public string GetBlockName(string edgeId)
        {
            return blockItemsByEdgeId.TryGetValue(edgeId, out FlowItem blockItem)
                ? blockItem.Definition.DisplayName
                : string.Empty;
        }
    }
}
