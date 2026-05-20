using System.Collections.Generic;
using UnityEngine;

namespace FOIA.Flow.Definitions
{
    [CreateAssetMenu(menuName = "FOIA/Flow/Edge Block Definition")]
    public sealed class EdgeBlockDefinition : ScriptableObject
    {
        [SerializeField] private string blockId;
        [SerializeField] private string displayName;
        [SerializeField] private List<FlowTagEffect> effects = new();

        public string BlockId => blockId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public IReadOnlyList<FlowTagEffect> Effects => effects;
    }
}
