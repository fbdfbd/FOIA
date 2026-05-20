using FOIA.Flow.Input;
using FOIA.Flow.Runtime;
using FOIA.Graph.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FOIA.Flow.Presentation
{
    public sealed class IntakeStaffSlotView : MonoBehaviour, IDropHandler
    {
        [SerializeField] private FoiaProcessSystem processSystem;
        [SerializeField] private StaffRuntimeStore staffStore;
        [SerializeField] private ProcessStateStore processState;
        [SerializeField] private TMP_Text label;

        private void Awake()
        {
            if (processSystem == null)
            {
                processSystem = GraphSceneLookup.FindFirst<FoiaProcessSystem>();
            }

            if (staffStore == null)
            {
                staffStore = GraphSceneLookup.FindFirst<StaffRuntimeStore>();
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
            if (FlowDragPayload.Is(FlowDragPayload.Staff))
            {
                processSystem.EquipStaff(FlowDragPayload.Id);
            }
        }

        private void Refresh()
        {
            if (label == null)
            {
                return;
            }

            if (processState == null || string.IsNullOrEmpty(processState.EquippedStaffId))
            {
                label.text = "직원 배치";
                return;
            }

            label.text = staffStore != null && staffStore.TryGetStaff(processState.EquippedStaffId, out StaffRuntime staff)
                ? staff.Definition.DisplayName
                : "직원 배치";
        }
    }
}
