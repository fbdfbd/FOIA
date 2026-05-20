using System;
using FOIA.Cards.Runtime;

namespace FOIA.Cards.Systems
{
    public sealed class CardPlacementSystem
    {
        public void MoveToInventory(IGameCardRuntime card)
        {
            Move(card, CardLocation.Inventory);
        }

        public void MoveToBoard(IGameCardRuntime card)
        {
            Move(card, CardLocation.Board);
        }

        private static void Move(IGameCardRuntime card, CardLocation location)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            card.SetLocation(location);
        }
    }
}
