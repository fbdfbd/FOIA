using System.Collections.Generic;
using FOIA.Core;
using UnityEngine;

namespace FOIA.Definitions
{
    [CreateAssetMenu(menuName = "FOIA/Definitions/Complaint")]
    public sealed class ComplaintDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private TagModifier[] initialTags;

        public ComplaintTypeId Id => new(id);
        public string DisplayName => displayName;
        public IReadOnlyList<TagModifier> InitialTags => initialTags;
    }
}
