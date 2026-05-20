using FOIA.Flow.Input;
using FOIA.Flow.Runtime;
using FOIA.Graph.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FOIA.Flow.Presentation
{
    public sealed class CraftSlotView : MonoBehaviour, IDropHandler, IPointerClickHandler
    {
        [SerializeField] private int slotIndex;
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
                processSystem.PutCraftItem(slotIndex, FlowDragPayload.Id);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                processSystem.ClearCraftItem(slotIndex);
            }
        }

        private void Refresh()
        {
            if (label == null || processState == null)
            {
                return;
            }

            FlowItem item = slotIndex == 0 ? processState.FirstCraftItem : processState.SecondCraftItem;
            label.text = item != null ? item.Definition.DisplayName : $"재료 {slotIndex + 1}";
        }
    }
}
