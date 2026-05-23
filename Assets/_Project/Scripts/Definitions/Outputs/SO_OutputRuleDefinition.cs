using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    [CreateAssetMenu(
        fileName = "SO_OutputRuleDefinition",
        menuName = "OneMoreSpoon/Definitions/Output Rule Definition")]
    public sealed class SO_OutputRuleDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string ruleId;

        [Header("Match")]
        [SerializeField] private SO_SubstanceDefinition requiredSubstance;
        [SerializeField] private List<string> requiredTags = new();
        [SerializeField] private List<string> requiredHistorySequence = new();

        [Header("Result")]
        [SerializeField] private SO_SubstanceDefinition resultSubstance;
        [SerializeField] private int resultAmount = 1;

        [Header("Byproducts")]
        [SerializeField] private List<OutputByproduct> byproducts = new();

        public string RuleId => ruleId;
        public SO_SubstanceDefinition RequiredSubstance => requiredSubstance;
        public IReadOnlyList<string> RequiredTags => requiredTags;
        public IReadOnlyList<string> RequiredHistorySequence => requiredHistorySequence;
        public SO_SubstanceDefinition ResultSubstance => resultSubstance;
        public int ResultAmount => Mathf.Max(1, resultAmount);
        public IReadOnlyList<OutputByproduct> Byproducts => byproducts;
    }
}
