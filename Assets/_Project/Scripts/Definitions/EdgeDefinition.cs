using System.Collections.Generic;
using FOIA.Core;
using UnityEngine;

namespace FOIA.Definitions
{
    [CreateAssetMenu(menuName = "FOIA/Definitions/Edge")]
    public sealed class EdgeDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private NodeDefinition from;
        [SerializeField] private NodeDefinition to;
        [SerializeField] private TagModifier[] baseModifiers;

        public EdgeId Id => new(id);
        public string DisplayName => displayName;
        public NodeDefinition From => from;
        public NodeDefinition To => to;
        public IReadOnlyList<TagModifier> BaseModifiers => baseModifiers;
    }
}
