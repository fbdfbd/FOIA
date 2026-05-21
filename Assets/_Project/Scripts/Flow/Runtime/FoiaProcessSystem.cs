using System.Collections.Generic;
using FOIA.Flow.Definitions;
using FOIA.Graph.Runtime;
using UnityEngine;

namespace FOIA.Flow.Runtime
{
    public sealed class FoiaProcessSystem : MonoBehaviour
    {
        private const string InventoryContainerId = "inventory";
        private const string BlocksContainerId = "blocks";

        [SerializeField] private int stressDecayPerTick = 5;

        [SerializeField] private FoiaFlowDatabase database;
        [SerializeField] private FlowRuntimeStore flowStore;
        [SerializeField] private StaffRuntimeStore staffStore;
        [SerializeField] private AgencyRuntimeStore agencyStore;
        [SerializeField] private ProcessStateStore processState;
        [SerializeField] private FlowLogStore logStore;
        [SerializeField] private EdgeBlockRuntimeStore edgeBlockStore;
        [SerializeField] private GraphRuntimeStore graphStore;
        [SerializeField] private NodeFlowStateStore nodeStateStore;

        private void Awake()
        {
            if (flowStore == null) flowStore = GraphSceneLookup.FindFirst<FlowRuntimeStore>();
            if (staffStore == null) staffStore = GraphSceneLookup.FindFirst<StaffRuntimeStore>();
            if (agencyStore == null) agencyStore = GraphSceneLookup.FindFirst<AgencyRuntimeStore>();
            if (processState == null) processState = GraphSceneLookup.FindFirst<ProcessStateStore>();
            if (logStore == null) logStore = GraphSceneLookup.FindFirst<FlowLogStore>();
            if (edgeBlockStore == null) edgeBlockStore = GraphSceneLookup.FindFirst<EdgeBlockRuntimeStore>();
            if (graphStore == null) graphStore = GraphSceneLookup.FindFirst<GraphRuntimeStore>();
            if (nodeStateStore == null) nodeStateStore = GraphSceneLookup.FindFirst<NodeFlowStateStore>();

            if (graphStore != null) graphStore.EdgeRemoved += OnEdgeRemoved;
        }

        private void OnDestroy()
        {
            if (graphStore != null) graphStore.EdgeRemoved -= OnEdgeRemoved;
        }

        private void OnEdgeRemoved(string edgeId)
        {
            FlowItem block = edgeBlockStore?.Unequip(edgeId);

            if (block == null)
            {
                return;
            }

            block.MoveToContainer(BlocksContainerId);
            flowStore?.NotifyItemChanged();
            AddLog($"{block.Definition.DisplayName} 블럭이 반환됐습니다.");
        }

        public void EquipStaff(string staffId)
        {
            processState?.EquipStaff(staffId);
        }

        public void EquipStaffOnNode(NodeEntity node, string staffId)
        {
            if (node == null || nodeStateStore == null || staffStore == null || !staffStore.TryGetStaff(staffId, out StaffRuntime staff))
            {
                return;
            }

            if (!staff.IsActive)
            {
                AddLog($"{staff.Definition.DisplayName} 직원은 더 이상 일할 수 없습니다.");
                return;
            }

            nodeStateStore.EquipStaff(node.NodeId, staffId);
            AddLog($"{staff.Definition.DisplayName} 직원을 접수 노드에 배치했습니다.");
        }

        public void EquipEdgeBlock(string itemId)
        {
            if (!TryGetItem(itemId, out FlowItem item) || item.Definition.Kind != FlowItemKind.EdgeBlock)
            {
                return;
            }

            FlowItem previous = processState != null ? processState.UnequipEdgeBlock() : null;

            if (previous != null)
            {
                previous.MoveToContainer(BlocksContainerId);
            }

            item.MoveToContainer(string.Empty);
            processState?.EquipEdgeBlock(item);
            flowStore.NotifyItemChanged();
            AddLog($"{item.Definition.DisplayName} 블럭을 장착했습니다.");
        }

        public void UnequipEdgeBlock()
        {
            FlowItem previous = processState != null ? processState.UnequipEdgeBlock() : null;

            if (previous == null)
            {
                return;
            }

            previous.MoveToContainer(BlocksContainerId);
            flowStore.NotifyItemChanged();
            AddLog($"{previous.Definition.DisplayName} 블럭을 해제했습니다.");
        }

        public void EquipEdgeBlockOnEdge(string edgeId, string itemId)
        {
            if (edgeBlockStore == null || !TryGetItem(itemId, out FlowItem item) || item.Definition.Kind != FlowItemKind.EdgeBlock)
            {
                return;
            }

            FlowItem previous = edgeBlockStore.Unequip(edgeId);

            if (previous != null)
            {
                previous.MoveToContainer(BlocksContainerId);
            }

            item.MoveToContainer(string.Empty);
            edgeBlockStore.Equip(edgeId, item);
            flowStore.NotifyItemChanged();
            AddLog($"{item.Definition.DisplayName} 블럭을 엣지에 장착했습니다.");
        }

        public void UnequipEdgeBlockFromEdge(string edgeId)
        {
            if (edgeBlockStore == null)
            {
                return;
            }

            FlowItem previous = edgeBlockStore.Unequip(edgeId);

            if (previous == null)
            {
                return;
            }

            previous.MoveToContainer(BlocksContainerId);
            flowStore.NotifyItemChanged();
            AddLog($"{previous.Definition.DisplayName} 블럭을 엣지에서 해제했습니다.");
        }

        public void StartIntake()
        {
            AddLog("접수는 접수 노드에 직원을 드롭한 뒤 접수 노드를 클릭해서 실행합니다.");
        }

        public void StartIntakeFromNode(NodeEntity intakeNode)
        {
            if (database == null || flowStore == null || staffStore == null || nodeStateStore == null || intakeNode == null)
            {
                return;
            }

            string staffId = nodeStateStore.GetStaffId(intakeNode.NodeId);

            if (string.IsNullOrEmpty(staffId) || !staffStore.TryGetStaff(staffId, out StaffRuntime staff))
            {
                AddLog("접수 노드에 직원을 먼저 배치해야 합니다.");
                return;
            }

            FlowItem document = flowStore.CreateItem(database.DefaultComplaint, string.Empty);
            document.AddTag(staff.Definition.TraitTag);
            document.AddTag($"staff:{staff.Definition.StaffId}");
            nodeStateStore.AddItem(intakeNode.NodeId, document);

            staff.AddStress(20);
            staffStore.NotifyChanged();
            nodeStateStore.ClearStaff(intakeNode.NodeId);

            AddLog($"{staff.Definition.DisplayName} 직원이 민원 서류를 접수 노드에 생성했습니다.");

            if (!staff.IsActive)
            {
                AddLog($"{staff.Definition.DisplayName} 직원이 스트레스를 견디지 못하고 퇴사했습니다.");
            }
        }

        public bool MoveFirstItemThroughEdge(EdgeRuntimeData edge)
        {
            if (edge == null || flowStore == null || nodeStateStore == null)
            {
                return false;
            }

            if (CanMoveForward(edge) && TryMoveFromNode(edge.FromNodeId, edge.ToNodeId, edge))
            {
                return true;
            }

            if (CanMoveBackward(edge) && TryMoveFromNode(edge.ToNodeId, edge.FromNodeId, edge))
            {
                return true;
            }

            AddLog("이 엣지로 이동시킬 민원 서류가 없습니다.");
            return false;
        }

        public void ProcessCurrentDocumentAtAgency(string agencyId)
        {
            AddLog("기관 처리는 기관 노드 안의 민원 서류를 기준으로 실행합니다.");
        }

        public void ProcessCurrentDocumentAtAgencyNode(NodeFlowData agencyNode)
        {
            ProcessAgencyNode(agencyNode);
        }

        public void ProcessAgencyNode(NodeFlowData agencyNode)
        {
            if (agencyNode == null || agencyNode.Agency == null || nodeStateStore == null || flowStore == null || database == null)
            {
                AddLog("기관 노드에 처리할 정보가 부족합니다.");
                return;
            }

            if (!agencyNode.TryGetComponent(out NodeEntity node) || !nodeStateStore.TryGetFirstItemId(node.NodeId, out string itemId))
            {
                AddLog("기관 노드에 처리할 민원 서류가 없습니다.");
                return;
            }

            if (!flowStore.TryGetItem(itemId, out FlowItem document))
            {
                return;
            }

            if (document.HasTag("processed"))
            {
                AddLog("이미 처리된 민원입니다. 결과 노드로 보내세요.");
                return;
            }

            if (!agencyStore.TryGetAgency(agencyNode.Agency.AgencyId, out AgencyRuntime agency))
            {
                return;
            }

            if (agency.Relationship < 40)
            {
                CreateByproduct(database.RefusalByproduct);
                document.AddTag("processed");
                agencyStore?.NotifyChanged();
                AddLog($"{agency.Definition.DisplayName} 기관이 비협조적으로 반려했습니다.");
                return;
            }

            FlowItemDefinition byproduct = FindMatchedAgencyByproduct(document, agency);
            CreateByproduct(byproduct != null ? byproduct : database.NormalByproduct);
            document.AddTag("processed");
            flowStore.NotifyItemChanged();
            agencyStore?.NotifyChanged();
            AddLog($"{agency.Definition.DisplayName} 기관이 민원을 처리했습니다.");
        }

        public void PutCraftItem(int slotIndex, string itemId)
        {
            if (!TryGetItem(itemId, out FlowItem item) || item.Definition.Kind == FlowItemKind.EdgeBlock)
            {
                return;
            }

            item.MoveToContainer(string.Empty);
            processState.SetCraftItem(slotIndex, item);
            flowStore.NotifyItemChanged();
        }

        public void ClearCraftItem(int slotIndex)
        {
            FlowItem item = slotIndex == 0 ? processState.FirstCraftItem : processState.SecondCraftItem;

            if (item == null)
            {
                return;
            }

            item.MoveToContainer(InventoryContainerId);
            processState.SetCraftItem(slotIndex, null);
            flowStore.NotifyItemChanged();
        }

        public void Craft()
        {
            FlowItem first = processState.FirstCraftItem;
            FlowItem second = processState.SecondCraftItem;

            if (first == null || second == null)
            {
                AddLog("조합 슬롯 두 칸을 모두 채워야 합니다.");
                return;
            }

            foreach (RecipeDefinition recipe in database.Recipes)
            {
                if (recipe != null && recipe.Matches(first.Definition, second.Definition))
                {
                    flowStore.DeleteItem(first.ItemId);
                    flowStore.DeleteItem(second.ItemId);
                    processState.ClearCraft();
                    flowStore.CreateItem(recipe.Result, BlocksContainerId);
                    AddLog(recipe.DiscoveryText);
                    return;
                }
            }

            flowStore.DeleteItem(first.ItemId);
            flowStore.DeleteItem(second.ItemId);
            processState.ClearCraft();
            AddLog("조합에 실패했습니다. 재료는 소모되었습니다.");
        }

        public void AutoTick()
        {
            if (nodeStateStore == null)
            {
                return;
            }

            IReadOnlyList<string> nodeIds = nodeStateStore.GetNodesWithItems();
            string[] snapshot = new string[nodeIds.Count];

            for (int i = 0; i < nodeIds.Count; i++)
            {
                snapshot[i] = nodeIds[i];
            }

            foreach (string nodeId in snapshot)
            {
                AutoTickNode(nodeId);
            }

            TickStressDecay();
        }

        private void TickStressDecay()
        {
            if (staffStore == null || stressDecayPerTick <= 0)
            {
                return;
            }

            bool anyChanged = false;

            foreach (StaffRuntime staff in staffStore.Staff)
            {
                if (staff.IsActive && staff.Stress > 0)
                {
                    staff.ReduceStress(stressDecayPerTick);
                    anyChanged = true;
                }
            }

            if (anyChanged)
            {
                staffStore.NotifyChanged();
            }
        }

        private void AutoTickNode(string nodeId)
        {
            if (graphStore == null || !graphStore.TryGetNode(nodeId, out NodeEntity node))
            {
                return;
            }

            if (!node.TryGetComponent(out NodeFlowData flowData))
            {
                return;
            }

            if (flowData.Role == NodeFlowRole.Output)
            {
                return;
            }

            if (flowData.Role == NodeFlowRole.Agency
                && nodeStateStore.TryGetFirstItemId(nodeId, out string itemId)
                && flowStore.TryGetItem(itemId, out FlowItem document)
                && !document.HasTag("processed"))
            {
                ProcessAgencyNode(flowData);
            }

            IReadOnlyList<EdgeRuntimeData> traversable = graphStore.GetTraversableEdgesFrom(nodeId);

            if (traversable.Count == 1)
            {
                MoveFirstItemThroughEdge(traversable[0]);
            }
        }

        public void CollectFirstOutputDocument(NodeEntity outputNode)
        {
            if (nodeStateStore == null || outputNode == null)
            {
                return;
            }

            if (!nodeStateStore.TryGetFirstItemId(outputNode.NodeId, out string itemId))
            {
                AddLog("결과물 노드에 수집할 문서가 없습니다.");
                return;
            }

            nodeStateStore.RemoveItem(itemId);

            if (flowStore != null && flowStore.TryGetItem(itemId, out FlowItem item))
            {
                item.MoveToContainer(InventoryContainerId);
                flowStore.NotifyItemChanged();
                AddLog($"{item.Definition.DisplayName} 문서를 부산물 인벤토리로 수집했습니다.");
            }
        }

        private bool TryMoveFromNode(string fromNodeId, string toNodeId, EdgeRuntimeData edge)
        {
            if (!nodeStateStore.TryGetFirstItemId(fromNodeId, out string itemId) || !flowStore.TryGetItem(itemId, out FlowItem item))
            {
                return false;
            }

            EdgeBlockDefinition block = edgeBlockStore != null ? edgeBlockStore.GetBlock(edge.EdgeId) : null;

            if (block != null)
            {
                item.AddTag($"block:{block.BlockId}");
                ApplyEdgeBlockEffects(item, block);
            }

            item.AddTag("edge_passed");
            item.AddTag($"edge:{edge.EdgeId}");
            nodeStateStore.AddItem(toNodeId, item);
            flowStore.NotifyItemChanged();
            AddLog($"{item.Definition.DisplayName} 문서가 엣지를 통해 다음 노드로 이동했습니다.");
            return true;
        }

        private bool TryGetItem(string itemId, out FlowItem item)
        {
            item = null;
            return flowStore != null && flowStore.TryGetItem(itemId, out item);
        }

        private FlowItemDefinition FindMatchedAgencyByproduct(FlowItem document, AgencyRuntime agency)
        {
            foreach (AgencyOutcomeDefinition outcome in database.AgencyOutcomes)
            {
                if (outcome == null || !document.HasTag(outcome.RequiredStaffTag))
                {
                    continue;
                }

                if (outcome.Matches(outcome.RequiredStaffTag, agency.Definition.TraitTag))
                {
                    AddLog(outcome.LogMessage);
                    return outcome.Byproduct;
                }
            }

            return null;
        }

        private void CreateByproduct(FlowItemDefinition definition)
        {
            if (definition != null)
            {
                flowStore.CreateItem(definition, InventoryContainerId);
            }
        }

        private static bool CanMoveForward(EdgeRuntimeData edge)
        {
            return edge.Direction == EdgeDirection.Forward
                || edge.Direction == EdgeDirection.Bidirectional
                || edge.Direction == EdgeDirection.Undirected;
        }

        private static bool CanMoveBackward(EdgeRuntimeData edge)
        {
            return edge.Direction == EdgeDirection.Backward
                || edge.Direction == EdgeDirection.Bidirectional
                || edge.Direction == EdgeDirection.Undirected;
        }

        private static void ApplyEdgeBlockEffects(FlowItem item, EdgeBlockDefinition block)
        {
            foreach (FlowTagEffect effect in block.Effects)
            {
                if (effect.Operation == FlowTagOperation.Remove)
                {
                    item.RemoveTag(effect.Tag);
                }
                else
                {
                    item.AddTag(effect.Tag);
                }
            }
        }

        private void AddLog(string message)
        {
            if (logStore != null)
            {
                logStore.Add(message);
            }
            else
            {
                Debug.Log(message);
            }
        }
    }
}
