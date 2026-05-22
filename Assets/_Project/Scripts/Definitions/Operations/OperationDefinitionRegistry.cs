using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    public sealed class OperationDefinitionRegistry
    {
        private readonly Dictionary<string, SO_OperationDefinition> definitions = new();

        public OperationDefinitionRegistry(IEnumerable<SO_OperationDefinition> definitions)
        {
            if (definitions == null)
                return;

            foreach (var definition in definitions)
            {
                if (definition == null)
                    continue;

                if (string.IsNullOrWhiteSpace(definition.OperationId))
                {
                    Debug.LogWarning($"Operation definition has empty id: {definition.name}");
                    continue;
                }

                if (this.definitions.ContainsKey(definition.OperationId))
                {
                    Debug.LogWarning($"Duplicate operation id ignored: {definition.OperationId}");
                    continue;
                }

                this.definitions.Add(definition.OperationId, definition);
            }
        }

        public bool TryGet(string operationId, out SO_OperationDefinition definition)
        {
            if (string.IsNullOrWhiteSpace(operationId))
            {
                definition = null;
                return false;
            }

            return definitions.TryGetValue(operationId, out definition);
        }
    }
}
