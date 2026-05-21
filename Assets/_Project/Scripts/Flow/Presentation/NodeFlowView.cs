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
    public sealed class NodeFlowView : MonoBehaviour, IDropHandler, IPointerClickHandler
    {
        [SerializeField] private FoiaProcessSystem processSystem;
        [SerializeField] private NodeFlowStateStore nodeStateStore;
        [SerializeField] private StaffRuntimeStore staffStore;
        [SerializeField] private TMP_Text label;
        [SerializeField] private float labelFontSize = 18f;

        private NodeEntity node;
        private NodeFlowData flowData;

        private void Awake()
        {
            node = GetComponent<NodeEntity>();
            flowData = GetComponent<NodeFlowData>();

            if (processSystem == null) processSystem = GraphSceneLookup.FindFirst<FoiaProcessSystem>();
            if (nodeStateStore == null) nodeStateStore = GraphSceneLookup.FindFirst<NodeFlowStateStore>();
            if (staffStore == null) staffStore = GraphSceneLookup.FindFirst<StaffRuntimeStore>();

            if (label == null)
            {
                label = GetComponentInChildren<TMP_Text>();
            }

            if (label == null)
            {
                label = CreateLabel((RectTransform)transform);
            }

            ApplyLabelFontSize();

            if (TryGetComponent(out Graphic graphic))
            {
                graphic.raycastTarget = true;
            }
        }

        private void OnValidate()
        {
            ApplyLabelFontSize();
        }

        private void OnEnable()
        {
            if (nodeStateStore != null)
            {
                nodeStateStore.StateChanged += Refresh;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (nodeStateStore != null)
            {
                nodeStateStore.StateChanged -= Refresh;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (flowData.Role == NodeFlowRole.Intake && FlowDragPayload.Is(FlowDragPayload.Staff))
            {
                processSystem.EquipStaffOnNode(node, FlowDragPayload.Id);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (flowData.Role == NodeFlowRole.Intake)
            {
                processSystem.StartIntakeFromNode(node);
                return;
            }

            if (flowData.Role == NodeFlowRole.Agency)
            {
                processSystem.ProcessAgencyNode(flowData);
                return;
            }

            if (flowData.Role == NodeFlowRole.Output)
            {
                processSystem.CollectFirstOutputDocument(node);
            }
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
                NodeFlowRole.Output => GetOutputText(),
                _ => "노드",
            };
        }

        private void ApplyLabelFontSize()
        {
            if (label != null)
            {
                label.fontSize = labelFontSize;
            }
        }

        private string GetIntakeText()
        {
            string staffId = nodeStateStore != null ? nodeStateStore.GetStaffId(node.NodeId) : string.Empty;
            int itemCount = nodeStateStore != null ? nodeStateStore.GetItemCount(node.NodeId) : 0;

            if (string.IsNullOrEmpty(staffId))
            {
                return $"접수\n직원 드롭\n보유 문서 {itemCount}";
            }

            string staffName = staffStore != null && staffStore.TryGetStaff(staffId, out StaffRuntime staff)
                ? staff.Definition.DisplayName
                : "직원";

            return $"접수\n{staffName}\n클릭: 민원 생산";
        }

        private string GetBoardText()
        {
            int itemCount = nodeStateStore != null ? nodeStateStore.GetItemCount(node.NodeId) : 0;
            return itemCount > 0
                ? $"대기열\n민원 서류 {itemCount}건\n엣지 클릭으로 이동"
                : "대기열\n민원 서류 없음";
        }

        private string GetAgencyText()
        {
            int itemCount = nodeStateStore != null ? nodeStateStore.GetItemCount(node.NodeId) : 0;
            string agencyName = flowData.Agency != null ? flowData.Agency.DisplayName : "기관";
            return itemCount > 0
                ? $"{agencyName}\n배정 문서 {itemCount}건\n클릭: 처리"
                : $"{agencyName}\n엣지로 문서 배정";
        }

        private string GetOutputText()
        {
            int itemCount = nodeStateStore != null ? nodeStateStore.GetItemCount(node.NodeId) : 0;
            return $"결과물\n도착 문서 {itemCount}건";
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
