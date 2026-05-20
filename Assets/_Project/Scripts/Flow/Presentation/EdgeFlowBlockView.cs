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
    [RequireComponent(typeof(EdgeEntity))]
    public sealed class EdgeFlowBlockView : MonoBehaviour, IDropHandler, IPointerClickHandler
    {
        [SerializeField] private FoiaProcessSystem processSystem;
        [SerializeField] private EdgeBlockRuntimeStore edgeBlockStore;
        [SerializeField] private TMP_Text label;

        private EdgeEntity edge;

        private void Awake()
        {
            edge = GetComponent<EdgeEntity>();

            if (processSystem == null)
            {
                processSystem = GraphSceneLookup.FindFirst<FoiaProcessSystem>();
            }

            if (edgeBlockStore == null)
            {
                edgeBlockStore = GraphSceneLookup.FindFirst<EdgeBlockRuntimeStore>();
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
            if (edgeBlockStore != null)
            {
                edgeBlockStore.BlocksChanged += Refresh;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (edgeBlockStore != null)
            {
                edgeBlockStore.BlocksChanged -= Refresh;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (edge != null && FlowDragPayload.Is(FlowDragPayload.Item))
            {
                processSystem.EquipEdgeBlockOnEdge(edge.EdgeId, FlowDragPayload.Id);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (edge != null && eventData.button == PointerEventData.InputButton.Left)
            {
                processSystem.UnequipEdgeBlockFromEdge(edge.EdgeId);
            }
        }

        public void Refresh()
        {
            if (label == null || edge == null || edgeBlockStore == null)
            {
                return;
            }

            string blockName = edgeBlockStore.GetBlockName(edge.EdgeId);
            label.text = string.IsNullOrEmpty(blockName) ? "+" : blockName;
        }

        private static TMP_Text CreateLabel(RectTransform parent)
        {
            GameObject textObject = new("EdgeBlockLabel", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            RectTransform rect = (RectTransform)textObject.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0.35f, 0f);
            rect.anchorMax = new Vector2(0.65f, 1f);
            rect.offsetMin = new Vector2(0f, -18f);
            rect.offsetMax = new Vector2(0f, 18f);

            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            FlowTextStyle.Apply(text);
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 16f;
            text.color = Color.yellow;
            text.raycastTarget = false;
            text.overflowMode = TextOverflowModes.Ellipsis;
            return text;
        }
    }
}
