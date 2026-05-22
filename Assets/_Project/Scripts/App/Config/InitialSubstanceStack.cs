using OneMoreSpoon.Game.Definitions;
using System;

namespace OneMoreSpoon.App.Config
{
    [Serializable]
    public sealed class InitialSubstanceStack
    {
        public SO_SubstanceDefinition SubstanceDefinition;
        public int Amount = 1;
        public bool IsInfinite;
    }
}
