using FOIA.Flow.Definitions;
using FOIA.Graph.Runtime;
using UnityEngine;

namespace FOIA.Flow.Runtime
{
    public sealed class FoiaProcessSystem : MonoBehaviour
    {
        private const string InventoryContainerId = "inventory";
        private const string BlocksContainerId = "blocks";

        [SerializeField] private FoiaFlowDatabase database;
        [SerializeField] private FlowRuntimeStore flowStore;
        [SerializeField] private StaffRuntimeStore staffStore;
        [SerializeField] private AgencyRuntimeStore agencyStore;
        [SerializeField] private ProcessStateStore processState;
        [SerializeField] private FlowLogStore logStore;
        [SerializeField] private EdgeBlockRuntimeStore edgeBlockStore;
        [SerializeField] private GraphRuntimeStore graphStore;

        private void Awake()
        {
            if (flowStore == null)
            {
                flowStore = GraphSceneLookup.FindFirst<FlowRuntimeStore>();
            }

            if (staffStore == null)
            {
                staffStore = GraphSceneLookup.FindFirst<StaffRuntimeStore>();
            }

            if (agencyStore == null)
            {
                agencyStore = GraphSceneLookup.FindFirst<AgencyRuntimeStore>();
            }

            if (processState == null)
            {
                processState = GraphSceneLookup.FindFirst<ProcessStateStore>();
            }

            if (logStore == null)
            {
                logStore = GraphSceneLookup.FindFirst<FlowLogStore>();
            }

            if (edgeBlockStore == null)
            {
                edgeBlockStore = GraphSceneLookup.FindFirst<EdgeBlockRuntimeStore>();
            }

            if (graphStore == null)
            {
                graphStore = GraphSceneLookup.FindFirst<GraphRuntimeStore>();
            }
        }

        public void EquipStaff(string staffId)
        {
            if (staffStore == null || processState == null || !staffStore.TryGetStaff(staffId, out StaffRuntime staff))
            {
                return;
            }

            if (!staff.IsActive)
            {
                AddLog($"{staff.Definition.DisplayName} 직원은 더 이상 일할 수 없습니다.");
                return;
            }

            processState.EquipStaff(staffId);
            AddLog($"{staff.Definition.DisplayName} 직원을 접수 노드에 배치했습니다.");
        }

        public void EquipEdgeBlock(string itemId)
        {
            if (!TryGetItem(itemId, out FlowItem item) || item.Definition.Kind != FlowItemKind.EdgeBlock)
            {
                return;
            }

            FlowItem previous = processState.UnequipEdgeBlock();

            if (previous != null)
            {
                previous.MoveToContainer(BlocksContainerId);
            }

            item.MoveToContainer(string.Empty);
            processState.EquipEdgeBlock(item);
            flowStore.NotifyItemChanged();
            AddLog($"{item.Definition.DisplayName} 블럭을 엣지에 장착했습니다.");
        }

        public void UnequipEdgeBlock()
        {
            FlowItem previous = processState.UnequipEdgeBlock();

            if (previous == null)
            {
                return;
            }

            previous.MoveToContainer(BlocksContainerId);
            flowStore.NotifyItemChanged();
            AddLog($"{previous.Definition.DisplayName} 블럭을 엣지에서 해제했습니다.");
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
            if (database == null || flowStore == null || staffStore == null || processState == null)
            {
                return;
            }

            if (processState.CurrentDocument != null)
            {
                AddLog("분기 보드에 처리 대기 중인 민원이 있습니다.");
                return;
            }

            if (string.IsNullOrEmpty(processState.EquippedStaffId)
                || !staffStore.TryGetStaff(processState.EquippedStaffId, out StaffRuntime staff))
            {
                AddLog("접수 노드에 직원을 먼저 배치해야 합니다.");
                return;
            }

            FlowItem document = flowStore.CreateItem(database.DefaultComplaint, string.Empty);
            document.AddTag(staff.Definition.TraitTag);
            document.AddTag($"staff:{staff.Definition.StaffId}");

            EdgeBlockDefinition block = processState.EquippedEdgeBlock;
            int stress = block != null && block.PreventsIntakeStress ? 0 : 20;

            if (block != null)
            {
                document.AddTag($"block:{block.BlockId}");
                ApplyEdgeBlockEffects(document, block);
            }

            staff.AddStress(stress);
            staffStore.NotifyChanged();

            if (!staff.IsActive)
            {
                processState.ClearStaff();
                AddLog($"{staff.Definition.DisplayName} 직원이 스트레스를 견디지 못하고 퇴사했습니다.");
            }
            else
            {
                processState.ClearStaff();
            }

            processState.SetCurrentDocument(document);
            AddLog($"{staff.Definition.DisplayName} 직원이 민원을 접수했습니다.");
        }

        public void StartIntakeFromNode(NodeEntity intakeNode)
        {
            if (database == null || flowStore == null || staffStore == null || processState == null || intakeNode == null)
            {
                return;
            }

            if (processState.CurrentDocument != null)
            {
                AddLog("분기 보드에 처리 대기 중인 민원이 있습니다.");
                return;
            }

            if (string.IsNullOrEmpty(processState.EquippedStaffId)
                || !staffStore.TryGetStaff(processState.EquippedStaffId, out StaffRuntime staff))
            {
                AddLog("접수 노드에 직원을 먼저 배치해야 합니다.");
                return;
            }

            FlowItem document = flowStore.CreateItem(database.DefaultComplaint, string.Empty);
            document.AddTag(staff.Definition.TraitTag);
            document.AddTag($"staff:{staff.Definition.StaffId}");

            EdgeBlockDefinition block = null;

            if (graphStore != null && graphStore.TryGetFirstNextNodeId(intakeNode.NodeId, out string nextNodeId, out string edgeId))
            {
                block = edgeBlockStore != null ? edgeBlockStore.GetBlock(edgeId) : null;
                document.MoveToNode(nextNodeId);
            }
            else if (TryFindNode(NodeFlowRole.Board, out NodeEntity boardNode))
            {
                document.MoveToNode(boardNode.NodeId);
            }

            int stress = block != null && block.PreventsIntakeStress ? 0 : 20;

            if (block != null)
            {
                document.AddTag($"block:{block.BlockId}");
                ApplyEdgeBlockEffects(document, block);
            }

            staff.AddStress(stress);
            staffStore.NotifyChanged();
            processState.ClearStaff();
            processState.SetCurrentDocument(document);

            AddLog($"{staff.Definition.DisplayName} 직원이 민원을 접수했습니다.");

            if (!staff.IsActive)
            {
                AddLog($"{staff.Definition.DisplayName} 직원이 스트레스를 견디지 못하고 퇴사했습니다.");
            }
        }

        public void ProcessCurrentDocumentAtAgency(string agencyId)
        {
            if (database == null || flowStore == null || agencyStore == null || processState == null)
            {
                return;
            }

            FlowItem document = processState.CurrentDocument;

            if (document == null)
            {
                AddLog("기관에 배정할 민원 문서가 없습니다.");
                return;
            }

            if (!agencyStore.TryGetAgency(agencyId, out AgencyRuntime agency))
            {
                return;
            }

            AddLog($"{agency.Definition.DisplayName} 기관에 민원을 배정했습니다.");

            EdgeBlockDefinition block = processState.EquippedEdgeBlock;

            if (block != null && block.ForcesAgencyApproval)
            {
                agency.AddRelationship(-block.ForcedApprovalRelationshipLoss);
                CreateByproduct(block.ForcedByproduct);
                FinishDocument($"{block.DisplayName} 효과로 기관이 강제 승인했습니다.");
                agencyStore.NotifyChanged();
                return;
            }

            if (agency.Relationship < 40)
            {
                CreateByproduct(database.RefusalByproduct);
                FinishDocument($"{agency.Definition.DisplayName} 기관이 비협조적으로 반려했습니다.");
                return;
            }

            FlowItemDefinition byproduct = FindMatchedAgencyByproduct(document, agency);
            CreateByproduct(byproduct != null ? byproduct : database.NormalByproduct);
            FinishDocument($"{agency.Definition.DisplayName} 기관이 민원을 처리했습니다.");
        }

        public void ProcessCurrentDocumentAtAgencyNode(NodeFlowData agencyNode)
        {
            if (agencyNode == null || agencyNode.Agency == null)
            {
                AddLog("기관 노드에 기관 데이터가 없습니다.");
                return;
            }

            ProcessCurrentDocumentAtAgency(agencyNode.Agency.AgencyId);
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

        private void FinishDocument(string message)
        {
            AddLog(message);

            if (processState.CurrentDocument != null)
            {
                flowStore.DeleteItem(processState.CurrentDocument.ItemId);
            }

            processState.ClearCurrentDocument();
            flowStore.NotifyItemChanged();
        }

        private static bool TryFindNode(NodeFlowRole role, out NodeEntity node)
        {
            NodeFlowData[] nodes = Object.FindObjectsByType<NodeFlowData>(FindObjectsSortMode.None);

            foreach (NodeFlowData data in nodes)
            {
                if (data.Role == role && data.TryGetComponent(out NodeEntity entity))
                {
                    node = entity;
                    return true;
                }
            }

            node = null;
            return false;
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
