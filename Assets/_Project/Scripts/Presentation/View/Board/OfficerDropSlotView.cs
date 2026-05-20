using R3;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using FOIA.Presentation.Views.Cards;

namespace FOIA.Presentation.Views.Board
{
    public sealed class OfficerDropSlotView : MonoBehaviour, IDropHandler
    {
        [SerializeField] private TextMeshProUGUI _assignedOfficerText;

        private readonly Subject<string> _onOfficerDropped = new();

        public Observable<string> OnOfficerDropped => _onOfficerDropped;

        public void SetAssignedOfficer(string officerId)
        {
            _assignedOfficerText.text = string.IsNullOrEmpty(officerId)
                ? "Officer: None"
                : $"Officer: {officerId}";
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (!TryGetPayload(eventData, out CardDragPayload payload))
            {
                return;
            }

            if (payload.Kind != DragKind.Officer)
            {
                return;
            }

            _onOfficerDropped.OnNext(payload.RuntimeId);
        }

        private static bool TryGetPayload(PointerEventData eventData, out CardDragPayload payload)
        {
            payload = default;

            if (eventData.pointerDrag == null)
            {
                return false;
            }

            ICardDragPayloadProvider provider = eventData.pointerDrag.GetComponent<ICardDragPayloadProvider>();

            if (provider == null)
            {
                return false;
            }

            payload = provider.Payload;
            return true;
        }
    }
}
