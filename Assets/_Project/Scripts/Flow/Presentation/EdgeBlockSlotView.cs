using FOIA.Flow.Input;
using FOIA.Flow.Runtime;
using FOIA.Graph.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FOIA.Flow.Presentation
{
    public sealed class EdgeBlockSlotView : MonoBehaviour, IDropHandler, IPointerClickHandler
    {
        [SerializeField] private FoiaProcessSystem processSystem;
        [SerializeField] private ProcessStateStore processState;
        [SerializeField] private TMP_Text label;

        private void Awake()
        {
            if (processSystem == null)
            {
                processSystem = GraphSceneLookup.FindFirst<FoiaProcessSystem>();
            }

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

        public void OnDrop(PointerEventData eventData)
        {
            if (FlowDragPayload.Is(FlowDragPayload.Item))
            {
                processSystem.EquipEdgeBlock(FlowDragPayload.Id);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                processSystem.UnequipEdgeBlock();
            }
        }

        private void Refresh()
        {
            if (label == null)
            {
                return;
            }

            label.text = processState != null && processState.EquippedEdgeBlockItem != null
                ? processState.EquippedEdgeBlockItem.Definition.DisplayName
                : "엣지 블럭";
        }
    }
}
