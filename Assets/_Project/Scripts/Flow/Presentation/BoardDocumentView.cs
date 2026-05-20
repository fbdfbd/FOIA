using FOIA.Flow.Input;
using FOIA.Flow.Runtime;
using FOIA.Graph.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FOIA.Flow.Presentation
{
    public sealed class BoardDocumentView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private ProcessStateStore processState;
        [SerializeField] private TMP_Text label;

        private void Awake()
        {
            if (processState == null)
            {
                processState = GraphSceneLookup.FindFirst<ProcessStateStore>();
            }

            if (label == null)
            {
                label = GetComponentInChildren<TMP_Text>();
            }
        }

        private void OnEnable()
        {
            if (processState != null)
            {
                processState.StateChanged += Refresh;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (processState != null)
            {
                processState.StateChanged -= Refresh;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (processState != null && processState.CurrentDocument != null)
            {
                FlowDragPayload.Begin(FlowDragPayload.Document, processState.CurrentDocument.ItemId);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            FlowDragPayload.Clear();
        }

        private void Refresh()
        {
            bool hasDocument = processState != null && processState.CurrentDocument != null;

            if (label == null)
            {
                return;
            }

            if (hasDocument)
            {
                label.text = processState.CurrentDocument.Definition.DisplayName;
                return;
            }

            label.text = "대기중인 민원 없음";
        }
    }
}
