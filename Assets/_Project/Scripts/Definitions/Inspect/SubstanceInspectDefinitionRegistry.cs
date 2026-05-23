using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    public sealed class SubstanceInspectDefinitionRegistry
    {
        private readonly Dictionary<string, SO_SubstanceInspectDefinition> definitions = new();

        public SubstanceInspectDefinitionRegistry(IEnumerable<SO_SubstanceInspectDefinition> definitions)
        {
            if (definitions == null)
                return;

            foreach (var definition in definitions)
            {
                if (definition == null)
                    continue;

                if (string.IsNullOrWhiteSpace(definition.TargetSubstanceId))
                {
                    Debug.LogWarning($"SubstanceInspectDefinition has empty targetSubstanceId: {definition.name}");
                    continue;
                }

                if (this.definitions.ContainsKey(definition.TargetSubstanceId))
                {
                    Debug.LogWarning($"Duplicate SubstanceInspectDefinition id ignored: {definition.TargetSubstanceId}");
                    continue;
                }

                this.definitions.Add(definition.TargetSubstanceId, definition);
            }
        }

        public bool TryGet(string targetSubstanceId, out SO_SubstanceInspectDefinition definition)
        {
            if (string.IsNullOrWhiteSpace(targetSubstanceId))
            {
                definition = null;
                return false;
            }

            return definitions.TryGetValue(targetSubstanceId, out definition);
        }
    }
}
