using System.Collections.Generic;
using FOIA.Core;
using UnityEngine;

namespace FOIA.Definitions
{
    [CreateAssetMenu(menuName = "FOIA/Definitions/Crafting Recipe")]
    public sealed class CraftingRecipeDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private ByproductAmount[] costs;
        [SerializeField] private EdgeBlockDefinition outputBlock;
        [SerializeField, Min(1)] private int outputAmount = 1;

        public string Id => id;
        public string DisplayName => displayName;
        public IReadOnlyList<ByproductAmount> Costs => costs;
        public EdgeBlockDefinition OutputBlock => outputBlock;
        public int OutputAmount => outputAmount;
    }
}
