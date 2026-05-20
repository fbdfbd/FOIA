using System.Collections.Generic;
using FOIA.Core;
using UnityEngine;

namespace FOIA.Definitions
{
    public enum StaffProfile
    {
        FastStressful = 0,
        SlowStable = 1,
        ProblemFinder = 2
    }

    [CreateAssetMenu(menuName = "FOIA/Definitions/Staff")]
    public sealed class StaffDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private StaffProfile profile;
        [SerializeField] private TagModifier[] tagModifiers;

        public StaffId Id => new(id);
        public string DisplayName => displayName;
        public StaffProfile Profile => profile;
        public IReadOnlyList<TagModifier> TagModifiers => tagModifiers;
    }
}
