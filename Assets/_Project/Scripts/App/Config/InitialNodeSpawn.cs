using OneMoreSpoon.Game.Definitions;
using System;
using UnityEngine;

namespace OneMoreSpoon.App.Config
{
    [Serializable]
    public sealed class InitialNodeSpawn
    {
        public SO_NodeDefinition Definition;
        public Vector2 Position;
    }
}
