using System.Collections.Generic;
using OneMoreSpoon.Game.Definitions;

namespace OneMoreSpoon.App.Encyclopedia
{
    public sealed class RecipeStepTextResolver
    {
        private const string NodePrefix = "node:";
        private const string EdgePrefix = "edge:";

        private readonly NodeDefinitionRegistry nodeDefinitions;
        private readonly Dictionary<string, string> edgeBlockNamesByTag = new();

        public RecipeStepTextResolver(
            NodeDefinitionRegistry nodeDefinitions,
            SubstanceDefinitionRegistry substanceDefinitions)
        {
            this.nodeDefinitions = nodeDefinitions;
            BuildEdgeBlockIndex(substanceDefinitions);
        }

        public string Resolve(string stepId)
        {
            if (string.IsNullOrWhiteSpace(stepId))
                return string.Empty;

            if (stepId.StartsWith(NodePrefix))
            {
                var nodeId = stepId.Substring(NodePrefix.Length);
                return nodeDefinitions.TryGet(nodeId, out var node)
                    ? node.DisplayName
                    : stepId;
            }

            if (stepId.StartsWith(EdgePrefix))
            {
                return edgeBlockNamesByTag.TryGetValue(stepId, out var edgeBlockName)
                    ? edgeBlockName
                    : stepId;
            }

            return stepId;
        }

        private void BuildEdgeBlockIndex(SubstanceDefinitionRegistry substanceDefinitions)
        {
            if (substanceDefinitions == null)
                return;

            foreach (var substance in substanceDefinitions.GetAll())
            {
                if (substance == null || substance.Kind != SubstanceKind.EdgeBlock)
                    continue;

                foreach (var tag in substance.AddedTags)
                {
                    if (string.IsNullOrWhiteSpace(tag))
                        continue;

                    if (!edgeBlockNamesByTag.ContainsKey(tag))
                        edgeBlockNamesByTag.Add(tag, substance.DisplayName);
                }
            }
        }
    }
}
