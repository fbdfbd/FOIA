using UnityEngine;

namespace FOIA.Flow.Definitions
{
    [CreateAssetMenu(menuName = "FOIA/Flow/Agency Definition")]
    public sealed class AgencyDefinition : ScriptableObject
    {
        [SerializeField] private string agencyId;
        [SerializeField] private string displayName;
        [SerializeField] private string traitTag;
        [SerializeField] private int startRelationship = 50;

        public string AgencyId => agencyId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string TraitTag => traitTag;
        public int StartRelationship => startRelationship;
    }
}
