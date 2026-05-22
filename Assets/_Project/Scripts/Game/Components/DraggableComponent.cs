namespace OneMoreSpoon.Game.Components
{
    public struct DraggableComponent
    {
        public bool CanDrag;

        public DraggableComponent(bool canDrag)
        {
            CanDrag = canDrag;
        }
    }
}