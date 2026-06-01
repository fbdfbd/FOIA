using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.Game.Rules;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Factories
{
    public sealed class EdgeFactory
    {
        private readonly GameWorld world;
        private readonly GameWorldChanges changes;

        public EdgeFactory(
            GameWorld world,
            GameWorldChanges changes)
        {
            this.world = world;
            this.changes = changes;
        }

        public bool TryCreateEdge(
            GameEntityId fromNodeId,
            GameEntityId toNodeId,
            SO_OperationDefinition operationDefinition,
            out GameEntityId edgeId)
        {
            edgeId = GameEntityId.Invalid;

            if (!fromNodeId.IsValid || !toNodeId.IsValid)
                return false;

            if (fromNodeId == toNodeId)
                return false;

            if (operationDefinition == null)
            {
                Debug.LogWarning($"[EdgeConnection] Rejected from={fromNodeId} to={toNodeId} reason=OperationDefinitionMissing");
                return false;
            }

            if (!world.Nodes.TryGetValue(fromNodeId, out var fromNode))
                return false;

            if (!world.Nodes.TryGetValue(toNodeId, out var toNode))
                return false;

            if (fromNode.Category == NodeCategory.Merge || toNode.Category == NodeCategory.Merge)
            {
                Debug.LogWarning($"[EdgeConnection] Rejected from={fromNodeId} to={toNodeId} reason=MergeNodeCannotConnect");
                return false;
            }

            if (!EdgeConnectionRule.CanConnect(fromNode.Category, toNode.Category, out var rejectReason))
            {
                Debug.LogWarning($"[EdgeConnection] Rejected from={fromNodeId} to={toNodeId} reason={rejectReason}");
                return false;
            }

            if (world.HasEdge(fromNodeId, toNodeId) || world.HasEdge(toNodeId, fromNodeId))
            {
                Debug.LogWarning($"[EdgeConnection] Rejected from={fromNodeId} to={toNodeId} reason=EdgeAlreadyExists");
                return false;
            }

            edgeId = world.CreateEdge(
                fromNodeId,
                toNodeId,
                operationDefinition.OperationId
            );
            changes.MarkEdgeChanged(edgeId);

            Debug.Log(
                $"Created edge: {fromNodeId} -> {toNodeId}, Operation: {operationDefinition.DisplayName}"
            );

            return true;
        }
    }
}
