using FOIA.Officers.Runtime;
using FOIA.Presentation.Views.Cards;
using UnityEngine;

namespace FOIA.Presentation.Views.Inventory
{
    public sealed class OfficerInventoryView : ViewBase
    {
        [SerializeField] private RectTransform _cardRoot;
        [SerializeField] private OfficerCardView _officerCardPrefab;

        public OfficerCardView CreateOfficerCard(OfficerRuntime officer)
        {
            OfficerCardView cardView = Instantiate(_officerCardPrefab, _cardRoot);
            cardView.SetOfficer(officer);
            return cardView;
        }
    }
}
