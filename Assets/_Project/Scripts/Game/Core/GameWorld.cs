using OneMoreSpoon.Game.Components;
using OneMoreSpoon.Game.Definitions;
using System.Collections.Generic;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Core
{
    public sealed class GameWorld
    {
        private int nextEntityId = 1;
        private readonly Queue<FlowSpawnRequest> flowSpawnRequests = new();

        public readonly Dictionary<EntityId, NodeComponent> Nodes = new();
        public readonly Dictionary<EntityId, PositionComponent> Positions = new();
        public readonly Dictionary<EntityId, TagComponent> Tags = new();
        public readonly Dictionary<EntityId, DraggableComponent> Draggables = new();
        public readonly Dictionary<GameEntityId, EdgeComponent> Edges = new();
        public readonly Dictionary<GameEntityId, EdgeStateComponent> EdgeStates = new();
        public readonly Dictionary<GameEntityId, EdgeBlockSlotComponent> EdgeBlockSlots = new();
        public readonly Dictionary<GameEntityId, SubstanceComponent> Substances = new();
        public readonly Dictionary<GameEntityId, FlowComponent> Flows = new();
        public readonly Dictionary<GameEntityId, FlowHistoryComponent> FlowHistories = new();
        public readonly Dictionary<GameEntityId, SubstanceStackComponent> SubstanceStacks = new();
        public readonly Dictionary<GameEntityId, MergeSlotComponent> MergeSlots = new();

        public EntityId CreateEntity()
        {
            return new EntityId(nextEntityId++);
        }

        public EntityId CreateNode(
            string definitionId,
            ProcessLayer processLayer,
            NodeCategory category,
            Vector2 position,
            IReadOnlyList<string> baseTags)
        {
            var entityId = CreateEntity();

            Nodes[entityId] = new NodeComponent(
                definitionId,
                processLayer,
                category
            );

            Positions[entityId] = new PositionComponent(position);
            Tags[entityId] = new TagComponent(baseTags);
            Draggables[entityId] = new DraggableComponent(true);

            if (category == NodeCategory.Merge)
                MergeSlots[entityId] = new MergeSlotComponent();

            return entityId;
        }

        public EntityId CreateEdge(
            EntityId fromNodeId,
            EntityId toNodeId,
            string operationDefinitionId)
         {
            var edgeId = CreateEntity();

            Edges[edgeId] = new EdgeComponent(
                fromNodeId,
                toNodeId,
                operationDefinitionId
            );

            EdgeStates[edgeId] = new EdgeStateComponent(false, false);
            EdgeBlockSlots[edgeId] = new EdgeBlockSlotComponent();

            return edgeId;
        }

        public EntityId CreateFlowEntity(
            string substanceId,
            EntityId startNodeId,
            IReadOnlyList<string> baseTags)
        {
            var entityId = CreateEntity();

            Substances[entityId] = new SubstanceComponent(substanceId);
            Flows[entityId] = new FlowComponent(startNodeId);
            FlowHistories[entityId] = new FlowHistoryComponent();
            Tags[entityId] = new TagComponent(baseTags);

            Debug.Log($"[Flow] Created entity={entityId} substance={substanceId} startNode={startNodeId} tags=[{string.Join(", ", baseTags)}]");

            return entityId;
        }

        public bool EnqueueFlowSpawn(EntityId targetNodeId, string substanceId)
        {
            if (!targetNodeId.IsValid)
            {
                Debug.LogWarning($"[FlowSpawn] Rejected substance={substanceId} reason=InvalidTargetNode");
                return false;
            }

            if (string.IsNullOrWhiteSpace(substanceId))
            {
                Debug.LogWarning($"[FlowSpawn] Rejected targetNode={targetNodeId} reason=EmptySubstanceId");
                return false;
            }

            if (!Nodes.ContainsKey(targetNodeId))
            {
                Debug.LogWarning($"[FlowSpawn] Rejected substance={substanceId} targetNode={targetNodeId} reason=NodeNotFound");
                return false;
            }

            flowSpawnRequests.Enqueue(new FlowSpawnRequest(targetNodeId, substanceId));
            Debug.Log($"[FlowSpawn] Enqueued substance={substanceId} targetNode={targetNodeId} pending={flowSpawnRequests.Count}");
            return true;
        }

        public bool TryDequeueFlowSpawn(out FlowSpawnRequest request)
        {
            if (flowSpawnRequests.Count <= 0)
            {
                request = default;
                return false;
            }

            request = flowSpawnRequests.Dequeue();
            return true;
        }

        public EntityId CreateSubstanceStack(
            string substanceId,
            int amount,
            bool isInfinite,
            Vector2 position)
        {
            var entityId = CreateEntity();

            SubstanceStacks[entityId] = new SubstanceStackComponent(substanceId, amount, isInfinite);
            Positions[entityId] = new PositionComponent(position);

            return entityId;
        }

        public bool HasEdge(GameEntityId fromNodeId, GameEntityId toNodeId)
        {
            foreach (var edge in Edges.Values)
            {
                if (edge.FromNodeId == fromNodeId && edge.ToNodeId == toNodeId)
                    return true;
            }

            return false;
        }
    }
}
