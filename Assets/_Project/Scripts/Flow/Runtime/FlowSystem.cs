using FOIA.Flow.Definitions;
using FOIA.Graph.Runtime;
using UnityEngine;

namespace FOIA.Flow.Runtime
{
    public sealed class FlowSystem : MonoBehaviour
    {
        [SerializeField] private GraphRuntimeStore graphStore;
        [SerializeField] private FlowRuntimeStore flowStore;
        [SerializeField] private EdgeFlowStateStore edgeFlowStateStore;
        [SerializeField] private FlowItemDefinition defaultComplaint;
        [SerializeField] private string intakeContainerId = "intake";
        [SerializeField] private string outputContainerId = "output";

        private void Awake()
        {
            if (graphStore == null)
            {
                graphStore = GraphSceneLookup.FindFirst<GraphRuntimeStore>();
            }

            if (flowStore == null)
            {
                flowStore = GraphSceneLookup.FindFirst<FlowRuntimeStore>();
            }

            if (edgeFlowStateStore == null)
            {
                edgeFlowStateStore = GraphSceneLookup.FindFirst<EdgeFlowStateStore>();
            }
        }

        public FlowItem CreateDefaultComplaint()
        {
            return CreateItem(defaultComplaint, intakeContainerId);
        }

        public FlowItem CreateItem(FlowItemDefinition definition, string containerId)
        {
            if (flowStore == null)
            {
                Debug.LogWarning("FlowSystem needs a FlowRuntimeStore.");
                return null;
            }

            return flowStore.CreateItem(definition, containerId);
        }

        public bool PutItemOnSelectedNode(string itemId)
        {
            if (graphStore == null || !graphStore.TryGetSelectedNode(out NodeEntity node))
            {
                Debug.LogWarning("Select a node before putting an item on it.");
                return false;
            }

            return PutItemOnNode(itemId, node.NodeId);
        }

        public bool PutSelectedItemOnSelectedNode()
        {
            if (flowStore == null || !flowStore.TryGetSelectedItem(out FlowItem item))
            {
                Debug.LogWarning("Select a flow item before putting it on a node.");
                return false;
            }

            return PutItemOnSelectedNode(item.ItemId);
        }

        public bool PutItemOnNode(string itemId, string nodeId)
        {
            if (!TryGetItem(itemId, out FlowItem item))
            {
                return false;
            }

            item.MoveToNode(nodeId);
            item.MoveToContainer(string.Empty);
            item.AddTag($"node:{nodeId}");
            flowStore.NotifyItemChanged();
            return true;
        }

        public bool MoveItemThroughSelectedEdge(string itemId)
        {
            if (graphStore == null || !graphStore.TryGetSelectedEdge(out EdgeRuntimeData edge))
            {
                Debug.LogWarning("Select an edge before moving an item.");
                return false;
            }

            return MoveItemThroughEdge(itemId, edge.EdgeId);
        }

        public bool MoveSelectedItemThroughSelectedEdge()
        {
            if (flowStore == null || !flowStore.TryGetSelectedItem(out FlowItem item))
            {
                Debug.LogWarning("Select a flow item before moving it through an edge.");
                return false;
            }

            return MoveItemThroughSelectedEdge(item.ItemId);
        }

        public bool MoveItemThroughEdge(string itemId, string edgeId)
        {
            if (!TryGetItem(itemId, out FlowItem item))
            {
                return false;
            }

            if (string.IsNullOrEmpty(item.CurrentNodeId))
            {
                Debug.LogWarning("Item must be on a node before it can move through an edge.");
                return false;
            }

            if (graphStore == null || !graphStore.TryGetNextNodeId(item.CurrentNodeId, edgeId, out string nextNodeId))
            {
                Debug.LogWarning("Selected edge cannot move this item from its current node.");
                return false;
            }

            item.AddTag("edge_passed");
            item.AddTag($"edge:{edgeId}");
            ApplyEdgeBlocks(item, edgeId);
            item.MoveToNode(nextNodeId);
            flowStore.NotifyItemChanged();
            return true;
        }

        public bool SendItemToOutput(string itemId)
        {
            if (!TryGetItem(itemId, out FlowItem item))
            {
                return false;
            }

            item.MoveToNode(string.Empty);
            item.MoveToContainer(outputContainerId);
            item.AddTag("output");
            flowStore.NotifyItemChanged();
            return true;
        }

        public bool SendSelectedItemToOutput()
        {
            if (flowStore == null || !flowStore.TryGetSelectedItem(out FlowItem item))
            {
                Debug.LogWarning("Select a flow item before sending it to output.");
                return false;
            }

            return SendItemToOutput(item.ItemId);
        }

        private bool TryGetItem(string itemId, out FlowItem item)
        {
            item = null;

            if (flowStore == null || string.IsNullOrEmpty(itemId) || !flowStore.TryGetItem(itemId, out item))
            {
                Debug.LogWarning($"Flow item not found: {itemId}");
                return false;
            }

            return true;
        }

        private void ApplyEdgeBlocks(FlowItem item, string edgeId)
        {
            if (edgeFlowStateStore == null)
            {
                return;
            }

            foreach (EdgeBlockDefinition block in edgeFlowStateStore.GetBlocks(edgeId))
            {
                if (block == null)
                {
                    continue;
                }

                item.AddTag($"block:{block.BlockId}");

                foreach (FlowTagEffect effect in block.Effects)
                {
                    ApplyTagEffect(item, effect);
                }
            }
        }

        private static void ApplyTagEffect(FlowItem item, FlowTagEffect effect)
        {
            if (effect == null)
            {
                return;
            }

            if (effect.Operation == FlowTagOperation.Remove)
            {
                item.RemoveTag(effect.Tag);
                return;
            }

            item.AddTag(effect.Tag);
        }
    }
}
