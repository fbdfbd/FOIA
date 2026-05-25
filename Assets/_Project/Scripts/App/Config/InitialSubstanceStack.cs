using OneMoreSpoon.Game.Definitions;
using System;
using UnityEngine;

namespace OneMoreSpoon.App.Config
{
    [Serializable]
    public sealed class InitialSubstanceStack
    {
        public SO_SubstanceDefinition SubstanceDefinition;
        public int Amount = 1;
        public bool IsInfinite;
        public Vector2 Position;
    }
}
