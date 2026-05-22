namespace OneMoreSpoon.Game.Components
{
    public struct EdgeStateComponent
    {
        public bool IsLocked;
        public bool IsCorrupted;

        public EdgeStateComponent(bool isLocked, bool isCorrupted)
        {
            IsLocked = isLocked;
            IsCorrupted = isCorrupted;
        }
    }
}