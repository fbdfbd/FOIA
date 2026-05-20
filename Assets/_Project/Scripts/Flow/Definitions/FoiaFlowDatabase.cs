using System.Collections.Generic;
using UnityEngine;

namespace FOIA.Flow.Definitions
{
    [CreateAssetMenu(menuName = "FOIA/Flow/Database")]
    public sealed class FoiaFlowDatabase : ScriptableObject
    {
        [SerializeField] private FlowItemDefinition defaultComplaint;
        [SerializeField] private List<StaffDefinition> staff = new();
        [SerializeField] private List<AgencyDefinition> agencies = new();
        [SerializeField] private List<AgencyOutcomeDefinition> agencyOutcomes = new();
        [SerializeField] private List<RecipeDefinition> recipes = new();
        [SerializeField] private FlowItemDefinition normalByproduct;
        [SerializeField] private FlowItemDefinition refusalByproduct;

        public FlowItemDefinition DefaultComplaint => defaultComplaint;
        public IReadOnlyList<StaffDefinition> Staff => staff;
        public IReadOnlyList<AgencyDefinition> Agencies => agencies;
        public IReadOnlyList<AgencyOutcomeDefinition> AgencyOutcomes => agencyOutcomes;
        public IReadOnlyList<RecipeDefinition> Recipes => recipes;
        public FlowItemDefinition NormalByproduct => normalByproduct;
        public FlowItemDefinition RefusalByproduct => refusalByproduct;
    }
}
