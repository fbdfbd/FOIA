using FOIA.Graph.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FOIA.Graph.Input
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EdgeEntity))]
    public sealed class EdgeContextInput : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GraphRuntimeStore graphStore;

        private EdgeEntity edge;

        public void Initialize(GraphRuntimeStore store, EdgeEntity edgeEntity)
        {
            graphStore = store;
            edge = edgeEntity;
        }

        private void Awake()
        {
            edge = GetComponent<EdgeEntity>();

            if (graphStore == null)
            {
                graphStore = GraphSceneLookup.FindFirst<GraphRuntimeStore>();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right
                || edge == null
                || graphStore == null
                || graphStore.SelectionType != GraphSelectionType.Edge
                || graphStore.SelectedEdgeId != edge.EdgeId)
            {
                return;
            }

            if (graphStore.DeleteEdge(edge.EdgeId))
            {
                Destroy(edge.gameObject);
            }
        }
    }
}
