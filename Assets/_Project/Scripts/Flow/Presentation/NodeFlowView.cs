using FOIA.Flow.Input;
using FOIA.Flow.Runtime;
using FOIA.Graph.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FOIA.Flow.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NodeEntity))]
    [RequireComponent(typeof(NodeFlowData))]
    public sealed class NodeFlowView : MonoBehaviour, IDropHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private FoiaProcessSystem processSystem;
        [SerializeField] private ProcessStateStore processState;
        [SerializeField] private StaffRuntimeStore staffStore;
        [SerializeField] private TMP_Text label;

        private NodeEntity node;
        private NodeFlowData flowData;

        private void Awake()
        {
            node = GetComponent<NodeEntity>();
            flowData = GetComponent<NodeFlowData>();

            if (processSystem == null)
            {
                processSystem = GraphSceneLookup.FindFirst<FoiaProcessSystem>();
            }

            if (processState == null)
            {
                processState = GraphSceneLookup.FindFirst<ProcessStateStore>();
            }

            if (staffStore == null)
            {
                staffStore = GraphSceneLookup.FindFirst<StaffRuntimeStore>();
            }

            if (label == null)
            {
                label = GetComponentInChildren<TMP_Text>();
            }

            if (label == null)
            {
                label = CreateLabel((RectTransform)transform);
            }

            if (TryGetComponent(out Graphic graphic))
            {
                graphic.raycastTarget = true;
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
            if (flowData.Role == NodeFlowRole.Intake && FlowDragPayload.Is(FlowDragPayload.Staff))
            {
                processSystem.EquipStaff(FlowDragPayload.Id);
                return;
            }

            if (flowData.Role == NodeFlowRole.Agency && FlowDragPayload.Is(FlowDragPayload.Document))
            {
                processSystem.ProcessCurrentDocumentAtAgencyNode(flowData);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && flowData.Role == NodeFlowRole.Intake)
            {
                processSystem.StartIntakeFromNode(node);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (flowData.Role != NodeFlowRole.Board || processState == null || processState.CurrentDocument == null)
            {
                return;
            }

            if (processState.CurrentDocument.CurrentNodeId == node.NodeId)
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

        public void Refresh()
        {
            if (label == null || flowData == null)
            {
                return;
            }

            label.text = flowData.Role switch
            {
                NodeFlowRole.Intake => GetIntakeText(),
                NodeFlowRole.Board => GetBoardText(),
                NodeFlowRole.Agency => GetAgencyText(),
                NodeFlowRole.Output => "결과물",
                _ => "노드",
            };
        }

        private string GetIntakeText()
        {
            if (processState == null || string.IsNullOrEmpty(processState.EquippedStaffId))
            {
                return "접수\n직원 드롭\n직원 배치 후 클릭";
            }

            return staffStore != null && staffStore.TryGetStaff(processState.EquippedStaffId, out StaffRuntime staff)
                ? $"접수\n{staff.Definition.DisplayName}\n클릭하면 민원 생성"
                : "접수";
        }

        private string GetBoardText()
        {
            if (processState != null
                && processState.CurrentDocument != null
                && processState.CurrentDocument.CurrentNodeId == node.NodeId)
            {
                return $"대기열\n{processState.CurrentDocument.Definition.DisplayName}\n기관 노드로 드래그";
            }

            return "대기열\n민원 서류 없음";
        }

        private string GetAgencyText()
        {
            return flowData.Agency != null
                ? $"{flowData.Agency.DisplayName}\n민원 서류 드롭"
                : "기관\n데이터 없음";
        }

        private static TMP_Text CreateLabel(RectTransform parent)
        {
            GameObject textObject = new("FlowLabel", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            RectTransform rect = (RectTransform)textObject.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(8f, 8f);
            rect.offsetMax = new Vector2(-8f, -8f);

            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            FlowTextStyle.Apply(text);
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 18f;
            text.color = Color.white;
            text.raycastTarget = false;
            return text;
        }
    }
}
