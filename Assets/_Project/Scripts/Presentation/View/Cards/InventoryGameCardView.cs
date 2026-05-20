namespace FOIA.Presentation.Views.Cards
{
    public sealed class InventoryGameCardView : GameCardViewBase
    {
        protected override CardDragPayload CreateDragPayload()
        {
            return new CardDragPayload(DragKind.GameCard, RuntimeId);
        }
    }
}
