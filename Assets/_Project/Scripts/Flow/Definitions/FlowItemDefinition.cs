using System.Collections.Generic;
using UnityEngine;

namespace FOIA.Flow.Definitions
{
    [CreateAssetMenu(menuName = "FOIA/Flow/Item Definition")]
    public sealed class FlowItemDefinition : ScriptableObject
    {
        [SerializeField] private string itemId;
        [SerializeField] private string displayName;
        [SerializeField] private FlowItemKind kind = FlowItemKind.Complaint;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private List<string> startTags = new();

        public string ItemId => itemId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public FlowItemKind Kind => kind;
        public Color Color => color;
        public IReadOnlyList<string> StartTags => startTags;
    }
}
