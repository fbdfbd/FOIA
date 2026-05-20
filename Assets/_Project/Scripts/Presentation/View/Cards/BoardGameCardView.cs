using FOIA.Cards.Runtime;
using FOIA.Presentation.Views.Board;
using R3;
using UnityEngine;

namespace FOIA.Presentation.Views.Cards
{
    public sealed class BoardGameCardView : GameCardViewBase
    {
        [SerializeField] private OfficerDropSlotView _officerDropSlot;

        public Observable<string> OnOfficerDropped => _officerDropSlot.OnOfficerDropped;

        protected override CardDragPayload CreateDragPayload()
        {
            return new CardDragPayload(DragKind.GameCard, RuntimeId);
        }

        public override void SetCard(IGameCardRuntime card)
        {
            base.SetCard(card);
        }

        public void SetAssignedOfficer(string officerId)
        {
            _officerDropSlot.SetAssignedOfficer(officerId);
        }
    }
}
