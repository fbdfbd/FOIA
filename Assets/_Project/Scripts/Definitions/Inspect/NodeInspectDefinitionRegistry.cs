using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    public sealed class NodeInspectDefinitionRegistry
    {
        private readonly Dictionary<string, SO_NodeInspectDefinition> definitions = new();

        public NodeInspectDefinitionRegistry(IEnumerable<SO_NodeInspectDefinition> definitions)
        {
            if (definitions == null)
                return;

            foreach (var definition in definitions)
            {
                if (definition == null)
                    continue;

                if (string.IsNullOrWhiteSpace(definition.TargetDefinitionId))
                {
                    Debug.LogWarning($"NodeInspectDefinition has empty targetDefinitionId: {definition.name}");
                    continue;
                }

                if (this.definitions.ContainsKey(definition.TargetDefinitionId))
                {
                    Debug.LogWarning($"Duplicate NodeInspectDefinition id ignored: {definition.TargetDefinitionId}");
                    continue;
                }

                this.definitions.Add(definition.TargetDefinitionId, definition);
            }
        }

        public bool TryGet(string targetDefinitionId, out SO_NodeInspectDefinition definition)
        {
            if (string.IsNullOrWhiteSpace(targetDefinitionId))
            {
                definition = null;
                return false;
            }

            return definitions.TryGetValue(targetDefinitionId, out definition);
        }
    }
}
