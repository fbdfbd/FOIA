using System;
using System.Collections.Generic;
using OneMoreSpoon.App.Encyclopedia.Data;
using OneMoreSpoon.Game.Definitions;

namespace OneMoreSpoon.App.Encyclopedia.Providers
{
    public sealed class EdgeBlockDetailProvider : IEncyclopediaDetailProvider
    {
        private readonly SubstanceInspectDefinitionRegistry inspectDefinitions;
        private readonly MergeRecipeRegistry mergeRecipes;

        public EdgeBlockDetailProvider(
            SubstanceInspectDefinitionRegistry inspectDefinitions,
            MergeRecipeRegistry mergeRecipes)
        {
            this.inspectDefinitions = inspectDefinitions;
            this.mergeRecipes = mergeRecipes;
        }

        public bool CanHandle(SubstanceKind kind) => SubstanceKindRules.IsEdgeBlockLike(kind);

        public EncyclopediaDetailData BuildData(SO_SubstanceDefinition definition)
        {
            inspectDefinitions.TryGet(definition.SubstanceId, out var inspectDef);
            mergeRecipes.FindByResult(definition.SubstanceId, out var recipe);

            var description = inspectDef?.Description ?? string.Empty;
            var inputNames = BuildInputNames(recipe);

            return new EdgeBlockDetailData(definition.DisplayName, description, inputNames);
        }

        private static IReadOnlyList<string> BuildInputNames(SO_MergeRecipeDefinition recipe)
        {
            if (recipe == null || recipe.InputSubstances.Count == 0)
                return Array.Empty<string>();

            var names = new List<string>();
            foreach (var substance in recipe.InputSubstances)
                if (substance != null)
                    names.Add(substance.DisplayName);

            return names;
        }
    }
}
