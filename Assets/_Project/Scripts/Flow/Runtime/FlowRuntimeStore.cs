using System;
using System.Collections.Generic;
using FOIA.Flow.Definitions;
using UnityEngine;

namespace FOIA.Flow.Runtime
{
    public sealed class FlowRuntimeStore : MonoBehaviour
    {
        private readonly Dictionary<string, FlowItem> itemsById = new();
        private readonly List<FlowItem> cachedItems = new();

        public event Action ItemsChanged;
        public event Action SelectionChanged;

        public string SelectedItemId { get; private set; }

        public IReadOnlyList<FlowItem> Items
        {
            get
            {
                cachedItems.Clear();
                cachedItems.AddRange(itemsById.Values);
                return cachedItems;
            }
        }

        public FlowItem CreateItem(FlowItemDefinition definition, string containerId)
        {
            if (definition == null)
            {
                Debug.LogWarning("FlowRuntimeStore needs a FlowItemDefinition.");
                return null;
            }

            FlowItem item = new(Guid.NewGuid().ToString("N"), definition, containerId);
            itemsById.Add(item.ItemId, item);
            SelectItem(item.ItemId);
            ItemsChanged?.Invoke();
            return item;
        }

        public bool TryGetItem(string itemId, out FlowItem item)
        {
            return itemsById.TryGetValue(itemId, out item);
        }

        public bool TryGetSelectedItem(out FlowItem item)
        {
            item = null;

            if (string.IsNullOrEmpty(SelectedItemId))
            {
                return false;
            }

            return TryGetItem(SelectedItemId, out item);
        }

        public void SelectItem(string itemId)
        {
            if (SelectedItemId == itemId || !itemsById.ContainsKey(itemId))
            {
                return;
            }

            SelectedItemId = itemId;
            SelectionChanged?.Invoke();
        }

        public bool DeleteItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId) || !itemsById.Remove(itemId))
            {
                return false;
            }

            if (SelectedItemId == itemId)
            {
                SelectedItemId = string.Empty;
                SelectionChanged?.Invoke();
            }

            ItemsChanged?.Invoke();
            return true;
        }

        public void NotifyItemChanged()
        {
            ItemsChanged?.Invoke();
        }
    }
}
