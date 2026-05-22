using UnityEngine;

namespace OneMoreSpoon.Game.Components
{
    public struct PositionComponent
    {
        public Vector2 Value;

        public PositionComponent(Vector2 value)
        {
            Value = value;
        }
    }
}