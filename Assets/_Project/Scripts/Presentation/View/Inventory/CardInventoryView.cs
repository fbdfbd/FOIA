using FOIA.Cards.Runtime;
using FOIA.Presentation.Views.Cards;
using UnityEngine;

namespace FOIA.Presentation.Views.Inventory
{
    public sealed class CardInventoryView : ViewBase
    {
        [SerializeField] private RectTransform _cardRoot;
        [SerializeField] private InventoryGameCardView _gameCardPrefab;

        public InventoryGameCardView CreateCard(IGameCardRuntime card)
        {
            InventoryGameCardView cardView = Instantiate(_gameCardPrefab, _cardRoot);
            cardView.SetCard(card);
            return cardView;
        }

        public void RemoveCard(InventoryGameCardView cardView)
        {
            if (cardView != null)
            {
                Destroy(cardView.gameObject);
            }
        }
    }
}
