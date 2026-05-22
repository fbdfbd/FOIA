using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    public sealed class MergeRecipeRegistry
    {
        private readonly List<SO_MergeRecipeDefinition> recipes = new();

        public MergeRecipeRegistry(IEnumerable<SO_MergeRecipeDefinition> recipes)
        {
            if (recipes == null)
                return;

            foreach (var recipe in recipes)
            {
                if (recipe == null)
                    continue;

                if (recipe.InputSubstances == null || recipe.InputSubstances.Count < 2)
                {
                    Debug.LogWarning($"Merge recipe ignored because it needs at least two inputs: {recipe.name}");
                    continue;
                }

                if (recipe.ResultSubstance == null)
                {
                    Debug.LogWarning($"Merge recipe ignored because result substance is missing: {recipe.name}");
                    continue;
                }

                this.recipes.Add(recipe);
            }
        }

        public bool TryGetMatch(
            IReadOnlyList<string> inputSubstanceIds,
            out SO_MergeRecipeDefinition recipe)
        {
            foreach (var candidate in recipes)
            {
                if (!Matches(candidate, inputSubstanceIds))
                    continue;

                recipe = candidate;
                return true;
            }

            recipe = null;
            return false;
        }

        private static bool Matches(
            SO_MergeRecipeDefinition recipe,
            IReadOnlyList<string> inputSubstanceIds)
        {
            if (inputSubstanceIds == null ||
                recipe.InputSubstances.Count != inputSubstanceIds.Count)
                return false;

            var counts = new Dictionary<string, int>();

            foreach (var substance in recipe.InputSubstances)
            {
                if (substance == null || string.IsNullOrWhiteSpace(substance.SubstanceId))
                    return false;

                if (!counts.TryAdd(substance.SubstanceId, 1))
                    counts[substance.SubstanceId]++;
            }

            foreach (var substanceId in inputSubstanceIds)
            {
                if (string.IsNullOrWhiteSpace(substanceId))
                    return false;

                if (!counts.TryGetValue(substanceId, out var count))
                    return false;

                if (count <= 1)
                    counts.Remove(substanceId);
                else
                    counts[substanceId] = count - 1;
            }

            return counts.Count == 0;
        }
    }
}
