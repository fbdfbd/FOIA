namespace FOIA.Presentation.Views.Cards
{
    public static class CardDragState
    {
        private static CardDragPayload _currentPayload;

        public static bool HasPayload { get; private set; }
        public static CardDragPayload CurrentPayload => _currentPayload;

        public static void Set(CardDragPayload payload)
        {
            _currentPayload = payload;
            HasPayload = true;
        }

        public static void Clear()
        {
            _currentPayload = default;
            HasPayload = false;
        }
    }
}
