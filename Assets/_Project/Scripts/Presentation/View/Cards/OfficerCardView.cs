using FOIA.Officers.Runtime;
using TMPro;
using UnityEngine;

namespace FOIA.Presentation.Views.Cards
{
    public sealed class OfficerCardView : DraggableCardViewBase
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _statusText;

        public string OfficerId { get; private set; }

        public void SetOfficer(OfficerRuntime officer)
        {
            OfficerId = officer.RuntimeId;
            _nameText.text = officer.Definition.DisplayName;
            _statusText.text = $"Stress {officer.Stress} / {officer.Status}";
        }

        protected override CardDragPayload CreateDragPayload()
        {
            return new CardDragPayload(DragKind.Officer, OfficerId);
        }
    }
}
