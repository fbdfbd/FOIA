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
        [SerializeField] private bool preventsIntakeStress;
        [SerializeField] private bool forcesAgencyApproval;
        [SerializeField] private int forcedApprovalRelationshipLoss;
        [SerializeField] private FlowItemDefinition forcedByproduct;

        public string BlockId => blockId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public IReadOnlyList<FlowTagEffect> Effects => effects;
        public bool PreventsIntakeStress => preventsIntakeStress;
        public bool ForcesAgencyApproval => forcesAgencyApproval;
        public int ForcedApprovalRelationshipLoss => forcedApprovalRelationshipLoss;
        public FlowItemDefinition ForcedByproduct => forcedByproduct;
    }
}
