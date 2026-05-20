using System.Collections.Generic;
using FOIA.Core;
using UnityEngine;

namespace FOIA.Definitions
{
    [CreateAssetMenu(menuName = "FOIA/Definitions/Edge Block")]
    public sealed class EdgeBlockDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private TagModifier[] tagModifiers;
        [SerializeField] private ByproductAmount[] byproducts;

        public EdgeBlockId Id => new(id);
        public string DisplayName => displayName;
        public IReadOnlyList<TagModifier> TagModifiers => tagModifiers;
        public IReadOnlyList<ByproductAmount> Byproducts => byproducts;
    }
}
