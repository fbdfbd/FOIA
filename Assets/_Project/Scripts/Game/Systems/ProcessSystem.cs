using OneMoreSpoon.App.Encyclopedia;
using OneMoreSpoon.App.Messaging;
using OneMoreSpoon.App.Rewards;
using OneMoreSpoon.Game.Components;
using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using System.Collections.Generic;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class ProcessSystem
    {
        private const float DefaultEdgeDuration = 5f;
        private const float MinEdgeDuration = 0.01f;
        private const float InputDepartureInterval = 1f;
        private static readonly Vector2 OutputStackOffset = new(0f, -1.2f);
        private static readonly Vector2 OutputStackSpacing = new(0.6f, 0f);
        private static readonly Vector2 OutputRuleRewardOffset = new(0f, -0.6f);

        private readonly GameWorld world;
        private readonly NodeDefinitionRegistry nodeDefinitionRegistry;
        private readonly SubstanceDefinitionRegistry substanceDefinitionRegistry;
        private readonly OperationDefinitionRegistry operationDefinitionRegistry;
        private readonly OutputRuleRegistry outputRuleRegistry;
        private readonly ToastMessageQueue toastMessageQueue;
        private readonly DiscoveryService discoveryService;
        private readonly FirstDiscoveryRewardService firstDiscoveryRewardService;
        private readonly List<GameEntityId> flowBuffer = new();
        private readonly Dictionary<GameEntityId, GameEntityId> lastAmbiguousRouteNodeByFlow = new();
        private readonly Dictionary<GameEntityId, float> inputDepartureCooldowns = new();
        private readonly List<GameEntityId> inputCooldownNodeBuffer = new();

        public ProcessSystem(
            GameWorld world,
            NodeDefinitionRegistry nodeDefinitionRegistry,
            SubstanceDefinitionRegistry substanceDefinitionRegistry,
            OperationDefinitionRegistry operationDefinitionRegistry,
            OutputRuleRegistry outputRuleRegistry,
            ToastMessageQueue toastMessageQueue,
            DiscoveryService discoveryService,
            FirstDiscoveryRewardService firstDiscoveryRewardService)
        {
            this.world = world;
            this.nodeDefinitionRegistry = nodeDefinitionRegistry;
            this.substanceDefinitionRegistry = substanceDefinitionRegistry;
            this.operationDefinitionRegistry = operationDefinitionRegistry;
            this.outputRuleRegistry = outputRuleRegistry;
            this.toastMessageQueue = toastMessageQueue;
            this.discoveryService = discoveryService;
            this.firstDiscoveryRewardService = firstDiscoveryRewardService;
        }

        public void Tick(float deltaTime)
        {
            SpawnQueuedFlows(deltaTime);
            UpdateInputDepartureCooldowns(deltaTime);
            RouteWaitingFlows(deltaTime);
            MoveFlows(deltaTime);
            ApplyArrivalEffects(deltaTime);
            ResolveOutputs(deltaTime);
        }

        private void UpdateInputDepartureCooldowns(float deltaTime)
        {
            inputCooldownNodeBuffer.Clear();

            foreach (GameEntityId nodeId in inputDepartureCooldowns.Keys)
                inputCooldownNodeBuffer.Add(nodeId);

            foreach (GameEntityId nodeId in inputCooldownNodeBuffer)
            {
                float remainingTime = inputDepartureCooldowns[nodeId] - deltaTime;

                if (remainingTime <= 0f)
                {
                    inputDepartureCooldowns.Remove(nodeId);
                    continue;
                }

                inputDepartureCooldowns[nodeId] = remainingTime;
            }
        }

        private void SpawnQueuedFlows(float deltaTime)
        {
            while (world.TryDequeueFlowSpawn(out var request))
            {
                if (!world.Nodes.TryGetValue(request.TargetNodeId, out var node))
                {
                    Debug.LogWarning($"[FlowSpawn] Skipped substance={request.SubstanceId} targetNode={request.TargetNodeId} reason=NodeNotFound");
                    continue;
                }

                if (node.Category != NodeCategory.Input)
                {
                    Debug.LogWarning($"[FlowSpawn] Skipped substance={request.SubstanceId} targetNode={request.TargetNodeId} reason=TargetIsNotInput category={node.Category}");
                    continue;
                }

                if (!substanceDefinitionRegistry.TryGet(request.SubstanceId, out var substanceDefinition))
                {
                    Debug.LogWarning($"[FlowSpawn] Skipped substance={request.SubstanceId} targetNode={request.TargetNodeId} reason=SubstanceDefinitionNotFound");
                    continue;
                }

                if (!SubstanceFlowSpawnRule.CanSpawnFlow(substanceDefinition.Kind))
                {
                    Debug.LogWarning($"[FlowSpawn] Skipped substance={request.SubstanceId} targetNode={request.TargetNodeId} reason=SubstanceCannotSpawnFlow kind={substanceDefinition.Kind}");
                    continue;
                }

                world.CreateFlowEntity(
                    substanceDefinition.SubstanceId,
                    request.TargetNodeId,
                    substanceDefinition.BaseTags
                );
            }
        }

        private void RouteWaitingFlows(float deltaTime)
        {
            flowBuffer.Clear();

            foreach (var pair in world.Flows)
            {
                if (pair.Value.State == FlowState.WaitingAtNode)
                    flowBuffer.Add(pair.Key);
            }

            flowBuffer.Sort((firstFlowId, secondFlowId) =>
                firstFlowId.Value.CompareTo(secondFlowId.Value));

            foreach (var flowEntityId in flowBuffer)
            {
                if (!world.Flows.TryGetValue(flowEntityId, out var flow))
                    continue;

                if (!world.Nodes.TryGetValue(flow.CurrentNodeId, out var node))
                    continue;

                if (node.Category == NodeCategory.Output)
                {
                    ApplyNodeEffects(flowEntityId, flow.CurrentNodeId);
                    flow.ArriveAtOutput(flow.CurrentNodeId);
                    world.Flows[flowEntityId] = flow;
                    Debug.Log($"[Flow] ArrivedAtOutput entity={flowEntityId} outputNode={flow.CurrentNodeId}");
                    continue;
                }

                if (!TryFindNextEdge(flowEntityId, flow.CurrentNodeId, out var edgeId))
                    continue;

                if (node.Category == NodeCategory.Input && inputDepartureCooldowns.ContainsKey(flow.CurrentNodeId))
                {
                    continue;
                }

                flow.BeginEdge(edgeId);
                world.Flows[flowEntityId] = flow;

                if (node.Category == NodeCategory.Input)
                    inputDepartureCooldowns[flow.CurrentNodeId] = InputDepartureInterval;

                Debug.Log($"[Flow] Route started entity={flowEntityId} fromNode={flow.CurrentNodeId} edge={edgeId}");
            }
        }

        private void MoveFlows(float deltaTime)
        {
            flowBuffer.Clear();

            foreach (var pair in world.Flows)
            {
                if (pair.Value.State == FlowState.MovingOnEdge)
                    flowBuffer.Add(pair.Key);
            }

            foreach (var flowEntityId in flowBuffer)
            {
                if (!world.Flows.TryGetValue(flowEntityId, out var flow))
                    continue;

                if (!world.Edges.TryGetValue(flow.CurrentEdgeId, out var edge))
                {
                    flow.ArriveAtNode(flow.CurrentNodeId);
                    world.Flows[flowEntityId] = flow;
                    continue;
                }

                float duration = GetEdgeDuration(edge);
                flow.Progress += deltaTime / duration;

                if (flow.Progress < 1f)
                {
                    world.Flows[flowEntityId] = flow;
                    continue;
                }

                if (!world.Nodes.TryGetValue(edge.ToNodeId, out var toNode))
                    continue;

                var completedEdgeId = flow.CurrentEdgeId;

                ApplyOperationEffects(flowEntityId, completedEdgeId);
                ApplyEdgeBlockEffects(flowEntityId, completedEdgeId);

                if (toNode.Category == NodeCategory.Output)
                {
                    flow.ArriveAtOutput(edge.ToNodeId);
                    ApplyNodeEffects(flowEntityId, edge.ToNodeId);
                    Debug.Log($"[Flow] ArrivedAtOutput entity={flowEntityId} outputNode={edge.ToNodeId} viaEdge={completedEdgeId}");
                }
                else
                {
                    flow.ArriveAtNode(edge.ToNodeId);
                    ApplyNodeEffects(flowEntityId, edge.ToNodeId);
                }

                world.Flows[flowEntityId] = flow;
            }
        }

        private void ApplyArrivalEffects(float deltaTime)
        {
        }

        private void ApplyOperationEffects(GameEntityId flowEntityId, GameEntityId edgeId)
        {
            if (!world.Edges.TryGetValue(edgeId, out var edge))
                return;

            if (!operationDefinitionRegistry.TryGet(edge.OperationDefinitionId, out var operationDefinition))
                return;

            AddFlowHistory(flowEntityId, $"operation:{edge.OperationDefinitionId}");
            AddFlowTags(flowEntityId, operationDefinition.OutputTags, $"Operation edge={edgeId} operation={edge.OperationDefinitionId}");
        }

        private void ApplyEdgeBlockEffects(GameEntityId flowEntityId, GameEntityId edgeId)
        {
            if (!world.Edges.TryGetValue(edgeId, out var edge))
                return;

            if (!IsForwardEdge(edge))
                return;

            if (!world.EdgeBlockSlots.TryGetValue(edgeId, out var slot) || !slot.HasBlock)
                return;

            foreach (var substanceId in slot.EquippedSubstanceIds)
                ApplyEdgeBlockEffect(flowEntityId, edgeId, substanceId);
        }

        private void ApplyEdgeBlockEffect(
            GameEntityId flowEntityId,
            GameEntityId edgeId,
            string substanceId)
        {
            if (!substanceDefinitionRegistry.TryGet(substanceId, out var blockDefinition))
                return;

            if (!SubstanceKindRules.CanEquipOnEdge(blockDefinition.Kind))
                return;

            AddFlowHistory(flowEntityId, $"edgeBlock:{substanceId}");
            AddFlowHistory(flowEntityId, blockDefinition.AddedTags);
            AddFlowTags(flowEntityId, blockDefinition.AddedTags, $"EdgeBlock edge={edgeId} block={substanceId}");
        }

        private void ApplyNodeEffects(GameEntityId flowEntityId, GameEntityId nodeId)
        {
            if (!world.Nodes.TryGetValue(nodeId, out var node))
                return;

            if (!nodeDefinitionRegistry.TryGet(node.DefinitionId, out var definition))
                return;

            AddFlowHistory(flowEntityId, $"node:{node.DefinitionId}");
            AddFlowTags(flowEntityId, definition.AddedFlowTags, $"Node node={nodeId} definition={node.DefinitionId}");
        }

        private bool IsForwardEdge(EdgeComponent edge)
        {
            if (!world.Nodes.TryGetValue(edge.FromNodeId, out var fromNode))
                return false;

            if (!world.Nodes.TryGetValue(edge.ToNodeId, out var toNode))
                return false;

            return fromNode.ProcessLayer <= toNode.ProcessLayer;
        }

        private void AddFlowTags(
            GameEntityId flowEntityId,
            IReadOnlyList<string> tags,
            string source)
        {
            if (tags == null || tags.Count <= 0)
                return;

            if (!world.Tags.TryGetValue(flowEntityId, out var flowTags))
                return;

            foreach (var tag in tags)
            {
                if (!flowTags.Add(tag))
                    continue;

                Debug.Log($"[FlowTag] Added entity={flowEntityId} source={source} tag={tag}");
            }
        }

        private void AddFlowHistory(GameEntityId flowEntityId, string entry)
        {
            if (!world.FlowHistories.TryGetValue(flowEntityId, out var history))
                return;

            history.Add(entry);
            Debug.Log($"[FlowHistory] Added entity={flowEntityId} entry={entry}");
        }

        private void AddFlowHistory(GameEntityId flowEntityId, IReadOnlyList<string> entries)
        {
            if (entries == null || entries.Count <= 0)
                return;

            foreach (var entry in entries)
                AddFlowHistory(flowEntityId, entry);
        }

        private void ResolveOutputs(float deltaTime)
        {
            flowBuffer.Clear();

            foreach (var pair in world.Flows)
            {
                if (pair.Value.State == FlowState.ArrivedAtOutput)
                    flowBuffer.Add(pair.Key);
            }

            for (int i = 0; i < flowBuffer.Count; i++)
                ResolveOutput(flowBuffer[i], i);
        }

        private void ResolveOutput(GameEntityId flowEntityId, int outputIndex)
        {
            if (!world.Flows.TryGetValue(flowEntityId, out var flow))
                return;

            if (flow.State != FlowState.ArrivedAtOutput)
                return;

            if (!world.Substances.TryGetValue(flowEntityId, out var substance))
            {
                Debug.LogWarning($"[Output] Failed entity={flowEntityId} outputNode={flow.CurrentNodeId} reason=SubstanceNotFound");
                ConsumeFlow(flowEntityId, flow);
                return;
            }

            if (!world.Positions.TryGetValue(flow.CurrentNodeId, out var outputPosition))
            {
                Debug.LogWarning($"[Output] Failed entity={flowEntityId} substance={substance.SubstanceId} outputNode={flow.CurrentNodeId} reason=OutputPositionNotFound");
                ConsumeFlow(flowEntityId, flow);
                return;
            }

            world.Tags.TryGetValue(flowEntityId, out var tags);
            world.FlowHistories.TryGetValue(flowEntityId, out var history);

            if (outputRuleRegistry.TryGetMatch(
                substance.SubstanceId,
                tags,
                history,
                discoveryService.IsEncountered,
                out var rule))
            {
                Debug.Log($"[Output] RuleMatched entity={flowEntityId} rule={rule.RuleId} substance={substance.SubstanceId}");
                CreateRuleOutputStacks(rule, outputPosition.Value, outputIndex);
            }
            else
            {
                Debug.LogWarning($"[Output] RuleMissing entity={flowEntityId} substance={substance.SubstanceId} tags=[{GetTagDebugText(tags)}]");
                CreateFallbackOutputStack(substance.SubstanceId, outputPosition.Value, outputIndex);
            }

            ConsumeFlow(flowEntityId, flow);
        }

        private void CreateRuleOutputStacks(
            SO_OutputRuleDefinition rule,
            Vector2 outputPosition,
            int outputIndex)
        {
            Vector2 resultPosition = GetOutputStackPosition(outputPosition, outputIndex, 0);
            discoveryService.NotifyEncountered(rule.ResultSubstance.SubstanceId);
            var resultStackId = world.CreateSubstanceStack(
                rule.ResultSubstance.SubstanceId,
                rule.ResultAmount,
                false,
                resultPosition);

            Debug.Log($"[Output] ResultCreated rule={rule.RuleId} substance={rule.ResultSubstance.SubstanceId} amount={rule.ResultAmount} stack={resultStackId}");

            firstDiscoveryRewardService.GrantForSubstance(rule.ResultSubstance.SubstanceId, resultPosition);
            firstDiscoveryRewardService.GrantForOutputRule(rule.RuleId, resultPosition + OutputRuleRewardOffset);

            int slotIndex = 1;
            foreach (var byproduct in rule.Byproducts)
            {
                if (byproduct == null || byproduct.Substance == null || byproduct.Amount <= 0)
                    continue;

                if (!CanCreateByproduct(byproduct))
                    continue;

                Vector2 byproductPosition = GetOutputStackPosition(outputPosition, outputIndex, slotIndex);
                discoveryService.NotifyEncountered(byproduct.Substance.SubstanceId);
                var byproductStackId = world.CreateSubstanceStack(
                    byproduct.Substance.SubstanceId,
                    byproduct.Amount,
                    false,
                    byproductPosition);

                Debug.Log($"[Output] ByproductCreated rule={rule.RuleId} substance={byproduct.Substance.SubstanceId} amount={byproduct.Amount} stack={byproductStackId}");
                firstDiscoveryRewardService.GrantForSubstance(byproduct.Substance.SubstanceId, byproductPosition);
                slotIndex++;
            }
        }

        private bool CanCreateByproduct(OutputByproduct byproduct)
        {
            foreach (var substanceId in byproduct.RequiredUndiscoveredSubstanceIds)
            {
                if (string.IsNullOrWhiteSpace(substanceId))
                    continue;

                if (discoveryService.IsEncountered(substanceId))
                    return false;
            }

            return true;
        }

        private void CreateFallbackOutputStack(
            string substanceId,
            Vector2 outputPosition,
            int outputIndex)
        {
            Vector2 stackPosition = GetOutputStackPosition(outputPosition, outputIndex, 0);
            discoveryService.NotifyEncountered(substanceId);
            var stackId = world.CreateSubstanceStack(substanceId, 1, false, stackPosition);

            Debug.Log($"[Output] ResultCreated rule=Fallback substance={substanceId} amount=1 stack={stackId}");
            firstDiscoveryRewardService.GrantForSubstance(substanceId, stackPosition);
        }

        private static Vector2 GetOutputStackPosition(
            Vector2 outputPosition,
            int outputIndex,
            int slotIndex)
        {
            return outputPosition
                + OutputStackOffset
                + OutputStackSpacing * outputIndex
                + OutputStackSpacing * slotIndex;
        }

        private static string GetTagDebugText(TagComponent tags)
        {
            if (tags == null)
                return string.Empty;

            return tags.ToDebugString();
        }

        private void ConsumeFlow(GameEntityId flowEntityId, FlowComponent flow)
        {
            flow.Consume();
            world.Flows[flowEntityId] = flow;
        }

        private bool TryFindNextEdge(
            GameEntityId flowEntityId,
            GameEntityId currentNodeId,
            out GameEntityId edgeId)
        {
            edgeId = GameEntityId.Invalid;

            foreach (var pair in world.Edges)
            {
                if (pair.Value.FromNodeId != currentNodeId)
                    continue;

                if (!CanEnterEdge(flowEntityId, pair.Key))
                    continue;

                if (edgeId.IsValid)
                {
                    NotifyAmbiguousRoute(flowEntityId, currentNodeId);
                    edgeId = GameEntityId.Invalid;
                    return false;
                }

                edgeId = pair.Key;
            }

            if (!edgeId.IsValid)
                return false;

            lastAmbiguousRouteNodeByFlow.Remove(flowEntityId);
            return true;
        }

        private void NotifyAmbiguousRoute(GameEntityId flowEntityId, GameEntityId currentNodeId)
        {
            if (lastAmbiguousRouteNodeByFlow.TryGetValue(flowEntityId, out var lastNodeId) &&
                lastNodeId == currentNodeId)
            {
                return;
            }

            lastAmbiguousRouteNodeByFlow[flowEntityId] = currentNodeId;
            toastMessageQueue.Enqueue("연결 경로가 2개 이상이라 이동을 멈췄습니다.");
            Debug.LogWarning($"[Flow] RouteBlocked entity={flowEntityId} node={currentNodeId} reason=MultipleOutgoingEdges");
        }

        private bool CanEnterEdge(GameEntityId flowEntityId, GameEntityId edgeId)
        {
            if (!world.Flows.ContainsKey(flowEntityId))
                return false;

            if (!world.Edges.TryGetValue(edgeId, out var edge))
                return false;

            if (!world.Nodes.ContainsKey(edge.ToNodeId))
                return false;

            if (world.EdgeStates.TryGetValue(edgeId, out var state) && state.IsLocked)
                return false;

            return true;
        }

        private float GetEdgeDuration(EdgeComponent edge)
        {
            if (operationDefinitionRegistry.TryGet(edge.OperationDefinitionId, out var operationDefinition))
                return UnityEngine.Mathf.Max(operationDefinition.Duration, MinEdgeDuration);

            return DefaultEdgeDuration;
        }
    }

    public static class SubstanceFlowSpawnRule
    {
        public static bool CanSpawnFlow(SubstanceKind kind)
        {
            return SubstanceKindRules.CanSpawnFlow(kind);
        }
    }
}
