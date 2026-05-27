using System;
using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    [Serializable]
    public sealed class OutputByproduct
    {
        [SerializeField] private SO_SubstanceDefinition substance;
        [SerializeField] private int amount = 1;
        [SerializeField] private List<string> requiredUndiscoveredSubstanceIds = new();

        public SO_SubstanceDefinition Substance => substance;
        public int Amount => Mathf.Max(0, amount);
        public IReadOnlyList<string> RequiredUndiscoveredSubstanceIds => requiredUndiscoveredSubstanceIds;
    }
}
