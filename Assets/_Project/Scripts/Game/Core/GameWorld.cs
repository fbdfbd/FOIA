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

        public readonly Dictionary<EntityId, NodeComponent> Nodes = new();
        public readonly Dictionary<EntityId, PositionComponent> Positions = new();
        public readonly Dictionary<EntityId, TagComponent> Tags = new();
        public readonly Dictionary<EntityId, DraggableComponent> Draggables = new();
        public readonly Dictionary<GameEntityId, EdgeComponent> Edges = new();
        public readonly Dictionary<GameEntityId, EdgeStateComponent> EdgeStates = new();

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

            return edgeId;
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