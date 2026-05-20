using System.Collections.Generic;
using FOIA.Core;
using UnityEngine;

namespace FOIA.Definitions
{
    public enum AgencyProfile
    {
        CooperativeSlow = 0,
        FastSensitive = 1
    }

    [CreateAssetMenu(menuName = "FOIA/Definitions/Agency")]
    public sealed class AgencyDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private AgencyProfile profile;
        [SerializeField] private TagModifier[] tagModifiers;
        [SerializeField, Min(0)] private int relationshipSensitivity;

        public AgencyId Id => new(id);
        public string DisplayName => displayName;
        public AgencyProfile Profile => profile;
        public IReadOnlyList<TagModifier> TagModifiers => tagModifiers;
        public int RelationshipSensitivity => relationshipSensitivity;
    }
}
