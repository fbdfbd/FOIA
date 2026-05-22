using OneMoreSpoon.Game.Core;
using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class ProcessSystem
    {
        private readonly GameWorld world;
        private bool created;

        public ProcessSystem(GameWorld world)
        {
            this.world = world;
        }

        public void Tick(float deltaTime)
        {
            if (created)
                return;

            created = true;

            var nodeId = world.CreateNode(
                "test_node",
                Vector2.zero,
                new List<string> { "Test", "Movable" }
            );

            Debug.Log($"Created node: {nodeId}");
        }
    }
}