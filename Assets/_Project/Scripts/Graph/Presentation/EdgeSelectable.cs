using FOIA.Graph.Runtime;
using FOIA.UI.Components;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FOIA.Graph.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIClickable))]
    [RequireComponent(typeof(EdgeEntity))]
    public sealed class EdgeSelectable : MonoBehaviour
    {
        [SerializeField] private GraphRuntimeStore graphStore;

        private EdgeEntity edge;
        private UIClickable clickable;

        public void Initialize(GraphRuntimeStore store, EdgeEntity edgeEntity)
        {
            graphStore = store;
            edge = edgeEntity;
        }

        private void Awake()
        {
            edge = GetComponent<EdgeEntity>();
            clickable = GetComponent<UIClickable>();

            if (graphStore == null)
            {
                graphStore = GraphSceneLookup.FindFirst<GraphRuntimeStore>();
            }
        }

        private void OnEnable()
        {
            clickable.Clicked.AddListener(OnClicked);
        }

        private void OnDisable()
        {
            clickable.Clicked.RemoveListener(OnClicked);
        }

        private void OnClicked(PointerEventData eventData)
        {
            if (edge == null || edge.Data == null)
            {
                return;
            }

            graphStore?.SelectEdge(edge.EdgeId);
            Debug.Log($"Edge clicked: {edge.EdgeId}");
        }
    }
}
