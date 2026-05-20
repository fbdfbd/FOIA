namespace FOIA.Presentation.Views.Cards
{
    public readonly struct CardDragPayload
    {
        public readonly DragKind Kind;
        public readonly string RuntimeId;

        public CardDragPayload(DragKind kind, string runtimeId)
        {
            Kind = kind;
            RuntimeId = runtimeId;
        }
    }
}
