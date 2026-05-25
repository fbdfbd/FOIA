using System;
using System.Collections.Generic;
using OneMoreSpoon.App.Encyclopedia;
using OneMoreSpoon.App.Encyclopedia.Data;
using OneMoreSpoon.Game.Definitions;

namespace OneMoreSpoon.App.Encyclopedia.Providers
{
    public sealed class DishDetailProvider : IEncyclopediaDetailProvider
    {
        private readonly SubstanceInspectDefinitionRegistry inspectDefinitions;
        private readonly OutputRuleRegistry outputRules;
        private readonly RecipeStepTextResolver recipeStepTextResolver;

        public DishDetailProvider(
            SubstanceInspectDefinitionRegistry inspectDefinitions,
            OutputRuleRegistry outputRules,
            RecipeStepTextResolver recipeStepTextResolver)
        {
            this.inspectDefinitions = inspectDefinitions;
            this.outputRules = outputRules;
            this.recipeStepTextResolver = recipeStepTextResolver;
        }

        public bool CanHandle(SubstanceKind kind) =>
            kind == SubstanceKind.Dish || kind == SubstanceKind.FinalDish;

        public EncyclopediaDetailData BuildData(SO_SubstanceDefinition definition)
        {
            inspectDefinitions.TryGet(definition.SubstanceId, out var inspectDef);
            outputRules.FindByResult(definition.SubstanceId, out var rule);

            var description = inspectDef?.Description ?? string.Empty;
            var inputName = rule?.RequiredSubstance?.DisplayName ?? string.Empty;
            var steps = ResolveStepNames(rule?.RequiredHistorySequence);
            var byproducts = BuildByproductNames(rule);

            return new DishDetailData(definition.DisplayName, description, inputName, steps, byproducts);
        }

        private IReadOnlyList<string> ResolveStepNames(IReadOnlyList<string> stepIds)
        {
            if (stepIds == null || stepIds.Count == 0)
                return Array.Empty<string>();

            var names = new List<string>();
            foreach (var id in stepIds)
            {
                names.Add(recipeStepTextResolver.Resolve(id));
            }
            return names;
        }

        private static IReadOnlyList<string> BuildByproductNames(SO_OutputRuleDefinition rule)
        {
            if (rule == null || rule.Byproducts.Count == 0)
                return Array.Empty<string>();

            var names = new List<string>();
            foreach (var byproduct in rule.Byproducts)
                if (byproduct.Substance != null)
                    names.Add($"{byproduct.Substance.DisplayName} x{byproduct.Amount}");

            return names;
        }
    }
}
