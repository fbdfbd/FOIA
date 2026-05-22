using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.View.Edges;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.View.Factories
{
    public sealed class EdgeBlockIndicatorViewFactory
    {
        private readonly EdgeBlockIndicatorView prefab;
        private readonly GameWorld world;
        private readonly SubstanceDefinitionRegistry definitionRegistry;

        public EdgeBlockIndicatorViewFactory(
            EdgeBlockIndicatorView prefab,
            GameWorld world,
            SubstanceDefinitionRegistry definitionRegistry)
        {
            this.prefab = prefab;
            this.world = world;
            this.definitionRegistry = definitionRegistry;
        }

        public EdgeBlockIndicatorView Create(GameEntityId edgeId)
        {
            if (prefab == null)
            {
                Debug.LogWarning("[EdgeBlockIndicator] Create skipped reason=PrefabMissing");
                return null;
            }

            if (!world.Edges.TryGetValue(edgeId, out var edge))
            {
                Debug.LogWarning($"[EdgeBlockIndicator] Create skipped edge={edgeId} reason=EdgeNotFound");
                return null;
            }

            Vector3 position = Vector3.zero;

            if (world.Positions.TryGetValue(edge.FromNodeId, out var fromPosition) &&
                world.Positions.TryGetValue(edge.ToNodeId, out var toPosition))
            {
                position = Vector2.Lerp(fromPosition.Value, toPosition.Value, 0.5f);
            }

            var view = Object.Instantiate(prefab, position, Quaternion.identity);
            view.Bind(edgeId, world);
            view.Initialize(definitionRegistry);

            return view;
        }
    }
}
