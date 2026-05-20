using FOIA.Graph.Presentation;
using UnityEngine;

namespace FOIA.Graph.Runtime
{
    public sealed class ConnectionSystem : MonoBehaviour
    {
        [SerializeField] private GraphRuntimeStore graphStore;
        [SerializeField] private UIEdgeFactory edgeFactory;
        [SerializeField] private bool preventDuplicateEdges = true;
        [SerializeField] private EdgeDuplicatePolicy duplicatePolicy = EdgeDuplicatePolicy.AnyDirection;
        [SerializeField] private EdgeDirection defaultDirection = EdgeDirection.Forward;

        private NodeEntity pendingNode;

        private void Awake()
        {
            if (graphStore == null)
            {
                graphStore = GraphSceneLookup.FindFirst<GraphRuntimeStore>();
            }

            if (edgeFactory == null)
            {
                edgeFactory = GraphSceneLookup.FindFirst<UIEdgeFactory>();
            }
        }

        public void HandleNodeRightClicked(NodeEntity node)
        {
            if (node == null)
            {
                return;
            }

            if (pendingNode == null)
            {
                pendingNode = node;
                Debug.Log($"Connection start: {node.name}");
                return;
            }

            if (pendingNode == node)
            {
                pendingNode = null;
                Debug.Log($"Connection canceled: {node.name}");
                return;
            }

            TryCreateConnection(pendingNode, node);
            pendingNode = null;
        }

        public void CancelPendingConnection()
        {
            pendingNode = null;
        }

        private void TryCreateConnection(NodeEntity fromNode, NodeEntity toNode)
        {
            if (graphStore == null)
            {
                Debug.LogWarning("ConnectionSystem needs a GraphRuntimeStore.");
                return;
            }

            if (edgeFactory == null)
            {
                Debug.LogWarning("ConnectionSystem needs a UIEdgeFactory.");
                return;
            }

            EdgeDuplicatePolicy resolvedDuplicatePolicy = preventDuplicateEdges
                ? duplicatePolicy
                : EdgeDuplicatePolicy.AllowParallel;

            if (graphStore.HasEdgeBetween(fromNode, toNode, resolvedDuplicatePolicy))
            {
                Debug.Log($"Connection already exists: {fromNode.name} -> {toNode.name}");
                return;
            }

            EdgeRuntimeData edge = graphStore.CreateEdge(fromNode, toNode, defaultDirection);
            edgeFactory.CreateEdge(edge, fromNode, toNode, graphStore);
            Debug.Log($"Connection created: {fromNode.name} -> {toNode.name}");
        }
    }
}
