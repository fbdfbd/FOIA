using OneMoreSpoon.Game.Components;
using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    public sealed class OutputRuleRegistry
    {
        private readonly List<SO_OutputRuleDefinition> rules = new();

        public OutputRuleRegistry(IEnumerable<SO_OutputRuleDefinition> rules)
        {
            if (rules == null)
                return;

            foreach (var rule in rules)
            {
                if (rule == null)
                    continue;

                if (rule.RequiredSubstance == null)
                {
                    Debug.LogWarning($"Output rule ignored because required substance is missing: {rule.name}");
                    continue;
                }

                if (rule.ResultSubstance == null)
                {
                    Debug.LogWarning($"Output rule ignored because result substance is missing: {rule.name}");
                    continue;
                }

                this.rules.Add(rule);
            }
        }

        public bool TryGetMatch(
            string substanceId,
            TagComponent tags,
            out SO_OutputRuleDefinition rule)
        {
            foreach (var candidate in rules)
            {
                if (!MatchesSubstance(candidate, substanceId))
                    continue;

                if (!MatchesTags(candidate, tags))
                    continue;

                rule = candidate;
                return true;
            }

            rule = null;
            return false;
        }

        private static bool MatchesSubstance(
            SO_OutputRuleDefinition rule,
            string substanceId)
        {
            return rule.RequiredSubstance != null
                && rule.RequiredSubstance.SubstanceId == substanceId;
        }

        private static bool MatchesTags(
            SO_OutputRuleDefinition rule,
            TagComponent tags)
        {
            foreach (var requiredTag in rule.RequiredTags)
            {
                if (string.IsNullOrWhiteSpace(requiredTag))
                    continue;

                if (tags == null || !tags.Has(requiredTag))
                    return false;
            }

            return true;
        }
    }
}
