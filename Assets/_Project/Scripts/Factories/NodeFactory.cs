using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Factories
{
    public sealed class NodeFactory
    {
        private readonly GameWorld world;

        public NodeFactory(GameWorld world)
        {
            this.world = world;
        }

        public GameEntityId CreateNode(SO_NodeDefinition definition, Vector2 position)
        {
            return world.CreateNode(
                definition.DefinitionId,
                definition.ProcessLayer,
                definition.Category,
                position,
                definition.BaseTags
            );
        }
    }
}