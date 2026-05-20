using UnityEngine;

namespace FOIA.Flow.Definitions
{
    [CreateAssetMenu(menuName = "FOIA/Flow/Staff Definition")]
    public sealed class StaffDefinition : ScriptableObject
    {
        [SerializeField] private string staffId;
        [SerializeField] private string displayName;
        [SerializeField] private string traitTag;
        [SerializeField] private int startStress;

        public string StaffId => staffId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string TraitTag => traitTag;
        public int StartStress => startStress;
    }
}
