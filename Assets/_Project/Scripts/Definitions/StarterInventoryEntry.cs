using System;
using UnityEngine;

namespace FOIA.Definitions
{
    [Serializable]
    public struct StarterInventoryEntry
    {
        public EdgeBlockDefinition block;
        [Min(1)] public int amount;
    }
}
