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

            this.rules.Sort(CompareSpecificity);
        }

        public bool TryGetMatch(
            string substanceId,
            TagComponent tags,
            FlowHistoryComponent history,
            out SO_OutputRuleDefinition rule)
        {
            foreach (var candidate in rules)
            {
                if (!MatchesSubstance(candidate, substanceId))
                    continue;

                if (!MatchesTags(candidate, tags))
                    continue;

                if (!MatchesHistorySequence(candidate, history))
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

        private static int CompareSpecificity(
            SO_OutputRuleDefinition left,
            SO_OutputRuleDefinition right)
        {
            var historyCompare = CountNonEmpty(right.RequiredHistorySequence)
                .CompareTo(CountNonEmpty(left.RequiredHistorySequence));
            if (historyCompare != 0)
                return historyCompare;

            var tagCompare = CountNonEmpty(right.RequiredTags)
                .CompareTo(CountNonEmpty(left.RequiredTags));
            if (tagCompare != 0)
                return tagCompare;

            return string.CompareOrdinal(left.RuleId, right.RuleId);
        }

        private static int CountNonEmpty(IReadOnlyList<string> values)
        {
            if (values == null)
                return 0;

            var count = 0;
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    count++;
            }

            return count;
        }

        private static bool MatchesHistorySequence(
            SO_OutputRuleDefinition rule,
            FlowHistoryComponent history)
        {
            var requiredSequence = rule.RequiredHistorySequence;
            if (requiredSequence == null || requiredSequence.Count <= 0)
                return true;

            if (history == null)
                return false;

            var matchIndex = 0;
            foreach (var entry in history.Entries)
            {
                while (matchIndex < requiredSequence.Count
                    && string.IsNullOrWhiteSpace(requiredSequence[matchIndex]))
                    matchIndex++;

                if (matchIndex < requiredSequence.Count && entry == requiredSequence[matchIndex])
                    matchIndex++;

                if (matchIndex >= requiredSequence.Count)
                    return true;
            }

            return false;
        }
    }
}
