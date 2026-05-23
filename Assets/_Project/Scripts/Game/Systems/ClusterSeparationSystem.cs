using OneMoreSpoon.Game.Components;
using OneMoreSpoon.Game.Core;
using System.Collections.Generic;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class ClusterSeparationSystem
    {
        private const float MinDistance = 1.15f;
        private const float PushStrength = 0.5f;
        private const float MaxStepPerIteration = 0.25f;
        private const int Iterations = 6;

        private readonly GameWorld world;
        private readonly PlayAreaBoundsSystem playAreaBoundsSystem;
        private readonly List<ClusterEntity> entities = new();

        public ClusterSeparationSystem(
            GameWorld world,
            PlayAreaBoundsSystem playAreaBoundsSystem)
        {
            this.world = world;
            this.playAreaBoundsSystem = playAreaBoundsSystem;
        }

        public void RelaxAround(GameEntityId anchorId)
        {
            if (!anchorId.IsValid)
                return;

            if (!world.Positions.ContainsKey(anchorId))
                return;

            CollectEntities();

            for (int i = 0; i < Iterations; i++)
                RelaxPairs();

            ApplyPositions();
        }

        private void CollectEntities()
        {
            entities.Clear();

            foreach (var pair in world.Positions)
            {
                if (!IsClusterTarget(pair.Key))
                    continue;

                entities.Add(new ClusterEntity(
                    pair.Key,
                    pair.Value.Value,
                    CanMove(pair.Key)
                ));
            }
        }

        private bool IsClusterTarget(GameEntityId entityId)
        {
            return world.Nodes.ContainsKey(entityId) ||
                world.SubstanceStacks.ContainsKey(entityId);
        }

        private bool CanMove(GameEntityId entityId)
        {
            if (world.Nodes.ContainsKey(entityId))
            {
                if (!world.Draggables.TryGetValue(entityId, out var draggable))
                    return false;

                if (!draggable.CanDrag)
                    return false;

                if (world.Tags.TryGetValue(entityId, out var tags) && tags.Has("Fixed"))
                    return false;
            }

            return true;
        }

        private void RelaxPairs()
        {
            for (int a = 0; a < entities.Count - 1; a++)
            {
                for (int b = a + 1; b < entities.Count; b++)
                    RelaxPair(a, b);
            }
        }

        private void RelaxPair(int aIndex, int bIndex)
        {
            var a = entities[aIndex];
            var b = entities[bIndex];

            if (!a.CanMove && !b.CanMove)
                return;

            Vector2 delta = a.Position - b.Position;
            float distance = delta.magnitude;

            if (distance >= MinDistance)
                return;

            Vector2 direction = distance > 0.001f
                ? delta / distance
                : GetFallbackDirection(a.Id, b.Id);

            float overlap = MinDistance - distance;
            Vector2 push = direction * Mathf.Min(overlap * PushStrength, MaxStepPerIteration);

            if (a.CanMove && b.CanMove)
            {
                a.Position = playAreaBoundsSystem.Clamp(a.Position + push * 0.5f);
                b.Position = playAreaBoundsSystem.Clamp(b.Position - push * 0.5f);
            }
            else if (a.CanMove)
            {
                a.Position = playAreaBoundsSystem.Clamp(a.Position + push);
            }
            else
            {
                b.Position = playAreaBoundsSystem.Clamp(b.Position - push);
            }

            entities[aIndex] = a;
            entities[bIndex] = b;
        }

        private static Vector2 GetFallbackDirection(GameEntityId a, GameEntityId b)
        {
            int seed = a.Value * 73856093 ^ b.Value * 19349663;
            float angle = (seed & 1023) / 1024f * Mathf.PI * 2f;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        private void ApplyPositions()
        {
            foreach (var entity in entities)
            {
                if (!entity.CanMove)
                    continue;

                world.Positions[entity.Id] = new PositionComponent(entity.Position);
            }
        }

        private struct ClusterEntity
        {
            public readonly GameEntityId Id;
            public readonly bool CanMove;
            public Vector2 Position;

            public ClusterEntity(GameEntityId id, Vector2 position, bool canMove)
            {
                Id = id;
                Position = position;
                CanMove = canMove;
            }
        }
    }
}
