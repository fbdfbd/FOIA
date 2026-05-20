using FOIA.Graph.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FOIA.Graph.Presentation
{
    [DisallowMultipleComponent]
    public sealed class GraphSelectionPanel : MonoBehaviour
    {
        [SerializeField] private GraphRuntimeStore graphStore;
        [SerializeField] private TMP_Text selectionText;

        public void Initialize(GraphRuntimeStore store, TMP_Text text)
        {
            graphStore = store;
            selectionText = text;
            Subscribe();
            RefreshSelection();
        }

        private void Awake()
        {
            if (graphStore == null)
            {
                graphStore = GraphSceneLookup.FindFirst<GraphRuntimeStore>();
            }
        }

        private void OnEnable()
        {
            Subscribe();
            RefreshSelection();
        }

        private void OnDisable()
        {
            if (graphStore != null)
            {
                graphStore.SelectionChanged -= RefreshSelection;
            }
        }

        private void Subscribe()
        {
            if (graphStore == null)
            {
                return;
            }

            graphStore.SelectionChanged -= RefreshSelection;
            graphStore.SelectionChanged += RefreshSelection;
        }

        private void RefreshSelection()
        {
            if (selectionText == null)
            {
                return;
            }

            if (graphStore == null || graphStore.SelectionType == GraphSelectionType.None)
            {
                selectionText.text = "No Selection";
                return;
            }

            selectionText.text = graphStore.SelectionType switch
            {
                GraphSelectionType.Node => $"Node\n{graphStore.SelectedNodeId}",
                GraphSelectionType.Edge => $"Edge\n{graphStore.SelectedEdgeId}",
                _ => "No Selection",
            };
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateDefaultPanel()
        {
            GraphRuntimeStore store = GraphSceneLookup.FindFirst<GraphRuntimeStore>();
            Canvas canvas = GraphSceneLookup.FindFirst<Canvas>();

            if (store == null || canvas == null || canvas.GetComponentInChildren<GraphSelectionPanel>(true) != null)
            {
                return;
            }

            GameObject panelObject = new("GraphSelectionPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(GraphSelectionPanel));
            RectTransform panelRect = (RectTransform)panelObject.transform;
            panelRect.SetParent(canvas.transform, false);
            panelRect.anchorMin = new Vector2(1f, 1f);
            panelRect.anchorMax = new Vector2(1f, 1f);
            panelRect.pivot = new Vector2(1f, 1f);
            panelRect.anchoredPosition = new Vector2(-24f, -24f);
            panelRect.sizeDelta = new Vector2(360f, 84f);

            Image panelImage = panelObject.GetComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.72f);
            panelImage.raycastTarget = false;

            GameObject textObject = new("SelectionText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            RectTransform textRect = (RectTransform)textObject.transform;
            textRect.SetParent(panelRect, false);
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(16f, 10f);
            textRect.offsetMax = new Vector2(-16f, -10f);

            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            text.raycastTarget = false;
            text.color = Color.white;
            text.fontSize = 20f;
            text.alignment = TextAlignmentOptions.MidlineLeft;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.overflowMode = TextOverflowModes.Ellipsis;

            panelObject.GetComponent<GraphSelectionPanel>().Initialize(store, text);
        }
    }
}
