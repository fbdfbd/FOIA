using UnityEngine;

namespace FOIA.Flow.Definitions
{
    [CreateAssetMenu(menuName = "FOIA/Flow/Agency Outcome Definition")]
    public sealed class AgencyOutcomeDefinition : ScriptableObject
    {
        [SerializeField] private string outcomeId;
        [SerializeField] private string requiredStaffTag;
        [SerializeField] private string requiredAgencyTraitTag;
        [SerializeField] private FlowItemDefinition byproduct;
        [TextArea]
        [SerializeField] private string logMessage;

        public string OutcomeId => outcomeId;
        public string RequiredStaffTag => requiredStaffTag;
        public string RequiredAgencyTraitTag => requiredAgencyTraitTag;
        public FlowItemDefinition Byproduct => byproduct;
        public string LogMessage => logMessage;

        public bool Matches(string staffTag, string agencyTraitTag)
        {
            return RequiredStaffTag == staffTag && RequiredAgencyTraitTag == agencyTraitTag;
        }
    }
}
