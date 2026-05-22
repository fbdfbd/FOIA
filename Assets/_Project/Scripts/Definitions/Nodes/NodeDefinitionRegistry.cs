using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    public sealed class NodeDefinitionRegistry
    {
        private readonly Dictionary<string, SO_NodeDefinition> definitions = new();

        public NodeDefinitionRegistry(IEnumerable<SO_NodeDefinition> definitions)
        {
            if (definitions == null)
                return;

            foreach (var definition in definitions)
            {
                if (definition == null)
                    continue;

                if (string.IsNullOrWhiteSpace(definition.DefinitionId))
                {
                    Debug.LogWarning($"Node definition has empty id: {definition.name}");
                    continue;
                }

                if (this.definitions.ContainsKey(definition.DefinitionId))
                {
                    Debug.LogWarning($"Duplicate node id ignored: {definition.DefinitionId}");
                    continue;
                }

                this.definitions.Add(definition.DefinitionId, definition);
            }
        }

        public bool TryGet(string definitionId, out SO_NodeDefinition definition)
        {
            if (string.IsNullOrWhiteSpace(definitionId))
            {
                definition = null;
                return false;
            }

            return definitions.TryGetValue(definitionId, out definition);
        }
    }
}
