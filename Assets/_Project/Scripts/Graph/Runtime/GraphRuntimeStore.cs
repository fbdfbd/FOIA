using System;
using System.Collections.Generic;
using UnityEngine;

namespace FOIA.Graph.Runtime
{
    public sealed class GraphRuntimeStore : MonoBehaviour
    {
        private readonly Dictionary<string, NodeEntity> nodesById = new();
        private readonly Dictionary<string, EdgeRuntimeData> edgesById = new();
        private readonly Dictionary<string, List<string>> edgeIdsByNodeId = new();

        public event Action SelectionChanged;

        public GraphSelectionType SelectionType { get; private set; }
        public string SelectedNodeId { get; private set; }
        public string SelectedEdgeId { get; private set; }

        public void RegisterNode(NodeEntity node)
        {
            if (node == null || string.IsNullOrEmpty(node.NodeId))
            {
                return;
            }

            nodesById[node.NodeId] = node;
        }

        public void UnregisterNode(NodeEntity node)
        {
            if (node == null || string.IsNullOrEmpty(node.NodeId))
            {
                return;
            }

            if (nodesById.TryGetValue(node.NodeId, out NodeEntity registeredNode) && registeredNode == node)
            {
                nodesById.Remove(node.NodeId);
            }
        }

        public bool HasEdgeBetween(
            NodeEntity first,
            NodeEntity second,
            EdgeDuplicatePolicy duplicatePolicy = EdgeDuplicatePolicy.AnyDirection)
        {
            if (first == null || second == null)
            {
                return false;
            }

            foreach (EdgeRuntimeData edge in edgesById.Values)
            {
                if (edge.MatchesDuplicate(first.NodeId, second.NodeId, duplicatePolicy))
                {
                    return true;
                }
            }

            return false;
        }

        public EdgeRuntimeData CreateEdge(
            NodeEntity fromNode,
            NodeEntity toNode,
            EdgeDirection direction = EdgeDirection.Forward)
        {
            if (fromNode == null)
            {
                throw new ArgumentNullException(nameof(fromNode));
            }

            if (toNode == null)
            {
                throw new ArgumentNullException(nameof(toNode));
            }

            EdgeRuntimeData edge = new(Guid.NewGuid().ToString("N"), fromNode.NodeId, toNode.NodeId, direction);
            edgesById.Add(edge.EdgeId, edge);
            AddNodeEdge(fromNode.NodeId, edge.EdgeId);
            AddNodeEdge(toNode.NodeId, edge.EdgeId);
            fromNode.SetConnected(true);
            toNode.SetConnected(true);
            return edge;
        }

        public bool SetEdgeDirection(string edgeId, EdgeDirection direction)
        {
            if (!edgesById.TryGetValue(edgeId, out EdgeRuntimeData edge))
            {
                return false;
            }

            edge.SetDirection(direction);
            return true;
        }

        public bool DeleteEdge(string edgeId)
        {
            if (string.IsNullOrEmpty(edgeId) || !edgesById.TryGetValue(edgeId, out EdgeRuntimeData edge))
            {
                return false;
            }

            edgesById.Remove(edgeId);
            RemoveNodeEdge(edge.FromNodeId, edgeId);
            RemoveNodeEdge(edge.ToNodeId, edgeId);
            RefreshNodeConnected(edge.FromNodeId);
            RefreshNodeConnected(edge.ToNodeId);

            if (SelectedEdgeId == edgeId)
            {
                ClearSelection();
            }

            return true;
        }

        public bool TryGetEdge(string edgeId, out EdgeRuntimeData edge)
        {
            return edgesById.TryGetValue(edgeId, out edge);
        }

        public bool TryGetNode(string nodeId, out NodeEntity node)
        {
            return nodesById.TryGetValue(nodeId, out node);
        }

        public bool TryGetSelectedNode(out NodeEntity node)
        {
            node = null;

            if (SelectionType != GraphSelectionType.Node || string.IsNullOrEmpty(SelectedNodeId))
            {
                return false;
            }

            return TryGetNode(SelectedNodeId, out node);
        }

        public bool TryGetSelectedEdge(out EdgeRuntimeData edge)
        {
            edge = null;

            if (SelectionType != GraphSelectionType.Edge || string.IsNullOrEmpty(SelectedEdgeId))
            {
                return false;
            }

            return TryGetEdge(SelectedEdgeId, out edge);
        }

        public bool TryGetNextNodeId(string currentNodeId, string edgeId, out string nextNodeId)
        {
            nextNodeId = string.Empty;

            if (string.IsNullOrEmpty(currentNodeId) || !edgesById.TryGetValue(edgeId, out EdgeRuntimeData edge))
            {
                return false;
            }

            if (CanMoveForward(edge, currentNodeId))
            {
                nextNodeId = edge.ToNodeId;
                return true;
            }

            if (CanMoveBackward(edge, currentNodeId))
            {
                nextNodeId = edge.FromNodeId;
                return true;
            }

            return false;
        }

        public bool TryGetFirstNextNodeId(string currentNodeId, out string nextNodeId, out string edgeId)
        {
            nextNodeId = string.Empty;
            edgeId = string.Empty;

            if (string.IsNullOrEmpty(currentNodeId) || !edgeIdsByNodeId.TryGetValue(currentNodeId, out List<string> nodeEdgeIds))
            {
                return false;
            }

            foreach (string currentEdgeId in nodeEdgeIds)
            {
                if (TryGetNextNodeId(currentNodeId, currentEdgeId, out nextNodeId))
                {
                    edgeId = currentEdgeId;
                    return true;
                }
            }

            return false;
        }

        public void SelectEdge(string edgeId)
        {
            if (!edgesById.ContainsKey(edgeId))
            {
                return;
            }

            if (SelectionType == GraphSelectionType.Edge && SelectedEdgeId == edgeId)
            {
                return;
            }

            if (!string.IsNullOrEmpty(SelectedEdgeId) && edgesById.TryGetValue(SelectedEdgeId, out EdgeRuntimeData previousEdge))
            {
                previousEdge.SetSelected(false);
            }

            SelectedNodeId = string.Empty;
            SelectedEdgeId = edgeId;
            SelectionType = GraphSelectionType.Edge;
            edgesById[edgeId].SetSelected(true);
            SelectionChanged?.Invoke();
        }

        public void SelectNode(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId) || !nodesById.ContainsKey(nodeId))
            {
                return;
            }

            if (SelectionType == GraphSelectionType.Node && SelectedNodeId == nodeId)
            {
                return;
            }

            if (!string.IsNullOrEmpty(SelectedEdgeId) && edgesById.TryGetValue(SelectedEdgeId, out EdgeRuntimeData previousEdge))
            {
                previousEdge.SetSelected(false);
            }

            SelectedNodeId = nodeId;
            SelectedEdgeId = string.Empty;
            SelectionType = GraphSelectionType.Node;
            SelectionChanged?.Invoke();
        }

        public void ClearSelection()
        {
            if (SelectionType == GraphSelectionType.None)
            {
                return;
            }

            if (edgesById.TryGetValue(SelectedEdgeId, out EdgeRuntimeData previousEdge))
            {
                previousEdge.SetSelected(false);
            }

            SelectedNodeId = string.Empty;
            SelectedEdgeId = string.Empty;
            SelectionType = GraphSelectionType.None;
            SelectionChanged?.Invoke();
        }

        private void AddNodeEdge(string nodeId, string edgeId)
        {
            if (!edgeIdsByNodeId.TryGetValue(nodeId, out List<string> edgeIds))
            {
                edgeIds = new List<string>();
                edgeIdsByNodeId.Add(nodeId, edgeIds);
            }

            edgeIds.Add(edgeId);
        }

        private void RemoveNodeEdge(string nodeId, string edgeId)
        {
            if (!edgeIdsByNodeId.TryGetValue(nodeId, out List<string> edgeIds))
            {
                return;
            }

            edgeIds.Remove(edgeId);

            if (edgeIds.Count == 0)
            {
                edgeIdsByNodeId.Remove(nodeId);
            }
        }

        private void RefreshNodeConnected(string nodeId)
        {
            if (nodesById.TryGetValue(nodeId, out NodeEntity node))
            {
                node.SetConnected(edgeIdsByNodeId.ContainsKey(nodeId));
            }
        }

        private static bool CanMoveForward(EdgeRuntimeData edge, string currentNodeId)
        {
            return edge.FromNodeId == currentNodeId
                && (edge.Direction == EdgeDirection.Forward
                    || edge.Direction == EdgeDirection.Bidirectional
                    || edge.Direction == EdgeDirection.Undirected);
        }

        private static bool CanMoveBackward(EdgeRuntimeData edge, string currentNodeId)
        {
            return edge.ToNodeId == currentNodeId
                && (edge.Direction == EdgeDirection.Backward
                    || edge.Direction == EdgeDirection.Bidirectional
                    || edge.Direction == EdgeDirection.Undirected);
        }
    }
}
