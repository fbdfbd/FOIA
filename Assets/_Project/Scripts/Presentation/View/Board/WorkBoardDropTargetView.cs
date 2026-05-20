using System;
using FOIA.Presentation.Views.Cards;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FOIA.Presentation.Views.Board
{
    public sealed class WorkBoardDropTargetView : MonoBehaviour, IDropHandler
    {
        private Action<string, PointerEventData> _onGameCardDropped;

        public void Initialize(Action<string, PointerEventData> onGameCardDropped)
        {
            _onGameCardDropped = onGameCardDropped;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
            {
                return;
            }

            ICardDragPayloadProvider provider = eventData.pointerDrag.GetComponent<ICardDragPayloadProvider>();

            if (provider == null)
            {
                return;
            }

            CardDragPayload payload = provider.Payload;

            if (payload.Kind != DragKind.GameCard)
            {
                return;
            }

            _onGameCardDropped?.Invoke(payload.RuntimeId, eventData);
        }
    }
}
