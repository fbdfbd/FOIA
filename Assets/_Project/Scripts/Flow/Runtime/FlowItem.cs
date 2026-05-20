using System;
using System.Collections.Generic;
using FOIA.Flow.Definitions;

namespace FOIA.Flow.Runtime
{
    public sealed class FlowItem
    {
        private readonly HashSet<string> tags = new();

        public FlowItem(string itemId, FlowItemDefinition definition, string containerId)
        {
            ItemId = itemId;
            Definition = definition;
            ContainerId = containerId;

            if (definition == null)
            {
                return;
            }

            foreach (string tag in definition.StartTags)
            {
                AddTag(tag);
            }
        }

        public string ItemId { get; }
        public FlowItemDefinition Definition { get; }
        public string CurrentNodeId { get; private set; }
        public string ContainerId { get; private set; }
        public IReadOnlyCollection<string> Tags => tags;

        public void MoveToNode(string nodeId)
        {
            CurrentNodeId = nodeId ?? string.Empty;
        }

        public void MoveToContainer(string containerId)
        {
            ContainerId = containerId ?? string.Empty;
        }

        public void AddTag(string tag)
        {
            if (!string.IsNullOrWhiteSpace(tag))
            {
                tags.Add(tag);
            }
        }

        public void RemoveTag(string tag)
        {
            if (!string.IsNullOrWhiteSpace(tag))
            {
                tags.Remove(tag);
            }
        }

        public bool HasTag(string tag)
        {
            return !string.IsNullOrWhiteSpace(tag) && tags.Contains(tag);
        }
    }
}
