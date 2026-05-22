using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    public sealed class SubstanceDefinitionRegistry
    {
        private readonly Dictionary<string, SO_SubstanceDefinition> definitions = new();

        public SubstanceDefinitionRegistry(IEnumerable<SO_SubstanceDefinition> definitions)
        {
            if (definitions == null)
                return;

            foreach (var definition in definitions)
            {
                if (definition == null)
                    continue;

                if (string.IsNullOrWhiteSpace(definition.SubstanceId))
                {
                    Debug.LogWarning($"Substance definition has empty id: {definition.name}");
                    continue;
                }

                if (this.definitions.ContainsKey(definition.SubstanceId))
                {
                    Debug.LogWarning($"Duplicate substance id ignored: {definition.SubstanceId}");
                    continue;
                }

                this.definitions.Add(definition.SubstanceId, definition);
            }
        }

        public bool TryGet(string substanceId, out SO_SubstanceDefinition definition)
        {
            if (string.IsNullOrWhiteSpace(substanceId))
            {
                definition = null;
                return false;
            }

            return definitions.TryGetValue(substanceId, out definition);
        }
    }
}
