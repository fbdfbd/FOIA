using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    [CreateAssetMenu(
        fileName = "SO_MergeRecipeDefinition",
        menuName = "OneMoreSpoon/Definitions/Merge Recipe Definition")]
    public sealed class SO_MergeRecipeDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string recipeId;

        [Header("Inputs")]
        [SerializeField] private List<SO_SubstanceDefinition> inputSubstances = new();

        [Header("Result")]
        [SerializeField] private SO_SubstanceDefinition resultSubstance;
        [SerializeField] private int resultAmount = 1;

        public string RecipeId => recipeId;
        public IReadOnlyList<SO_SubstanceDefinition> InputSubstances => inputSubstances;
        public SO_SubstanceDefinition ResultSubstance => resultSubstance;
        public int ResultAmount => Mathf.Max(1, resultAmount);
    }
}
