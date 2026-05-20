using UnityEngine;

namespace FOIA.Flow.Definitions
{
    [CreateAssetMenu(menuName = "FOIA/Flow/Recipe Definition")]
    public sealed class RecipeDefinition : ScriptableObject
    {
        [SerializeField] private string recipeId;
        [SerializeField] private FlowItemDefinition firstIngredient;
        [SerializeField] private FlowItemDefinition secondIngredient;
        [SerializeField] private FlowItemDefinition result;
        [TextArea]
        [SerializeField] private string discoveryText;

        public string RecipeId => recipeId;
        public FlowItemDefinition FirstIngredient => firstIngredient;
        public FlowItemDefinition SecondIngredient => secondIngredient;
        public FlowItemDefinition Result => result;
        public string DiscoveryText => discoveryText;

        public bool Matches(FlowItemDefinition first, FlowItemDefinition second)
        {
            return firstIngredient == first && secondIngredient == second
                || firstIngredient == second && secondIngredient == first;
        }
    }
}
