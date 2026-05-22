using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Factories
{
    public sealed class EdgeFactory
    {
        private readonly GameWorld world;

        public EdgeFactory(GameWorld world)
        {
            this.world = world;
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
                return false;

            if (!world.Nodes.TryGetValue(fromNodeId, out var fromNode))
                return false;

            if (!world.Nodes.TryGetValue(toNodeId, out var toNode))
                return false;

            if (fromNode.Category == NodeCategory.Merge || toNode.Category == NodeCategory.Merge)
            {
                Debug.LogWarning($"[EdgeConnection] Rejected from={fromNodeId} to={toNodeId} reason=MergeNodeCannotConnect");
                return false;
            }

            if (world.HasEdge(fromNodeId, toNodeId))
                return false;

            edgeId = world.CreateEdge(
                fromNodeId,
                toNodeId,
                operationDefinition.OperationId
            );

            Debug.Log(
                $"Created edge: {fromNodeId} -> {toNodeId}, Operation: {operationDefinition.DisplayName}"
            );

            return true;
        }
    }
}
