using FOIA.Graph.Runtime;
using FOIA.UI.Components;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FOIA.Graph.Input
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NodeEntity))]
    [RequireComponent(typeof(UIClickable))]
    public sealed class NodeSelectionInput : MonoBehaviour
    {
        [SerializeField] private GraphRuntimeStore graphStore;

        private NodeEntity node;
        private UIClickable clickable;

        public void Initialize(GraphRuntimeStore store, NodeEntity nodeEntity)
        {
            graphStore = store;
            node = nodeEntity;
        }

        private void Awake()
        {
            node = GetComponent<NodeEntity>();
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
            if (node == null)
            {
                return;
            }

            graphStore?.SelectNode(node.NodeId);
        }
    }
}
