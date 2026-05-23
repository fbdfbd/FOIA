using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    [CreateAssetMenu(
        fileName = "SO_DefinitionCatalog",
        menuName = "OneMoreSpoon/Definitions/Definition Catalog")]
    public sealed class SO_DefinitionCatalog : ScriptableObject
    {
        [Header("Definitions")]
        [SerializeField] private List<SO_NodeDefinition> nodeDefinitions = new();
        [SerializeField] private List<SO_OperationDefinition> operationDefinitions = new();
        [SerializeField] private List<SO_SubstanceDefinition> substanceDefinitions = new();
        [SerializeField] private List<SO_OutputRuleDefinition> outputRuleDefinitions = new();
        [SerializeField] private List<SO_MergeRecipeDefinition> mergeRecipeDefinitions = new();

        [Header("Inspect Definitions")]
        [SerializeField] private List<SO_NodeInspectDefinition> nodeInspectDefinitions = new();
        [SerializeField] private List<SO_SubstanceInspectDefinition> substanceInspectDefinitions = new();

        public IReadOnlyList<SO_NodeDefinition> NodeDefinitions => nodeDefinitions;
        public IReadOnlyList<SO_OperationDefinition> OperationDefinitions => operationDefinitions;
        public IReadOnlyList<SO_SubstanceDefinition> SubstanceDefinitions => substanceDefinitions;
        public IReadOnlyList<SO_OutputRuleDefinition> OutputRuleDefinitions => outputRuleDefinitions;
        public IReadOnlyList<SO_MergeRecipeDefinition> MergeRecipeDefinitions => mergeRecipeDefinitions;
        public IReadOnlyList<SO_NodeInspectDefinition> NodeInspectDefinitions => nodeInspectDefinitions;
        public IReadOnlyList<SO_SubstanceInspectDefinition> SubstanceInspectDefinitions => substanceInspectDefinitions;
    }
}
