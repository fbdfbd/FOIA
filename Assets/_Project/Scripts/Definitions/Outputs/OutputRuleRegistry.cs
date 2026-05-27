using OneMoreSpoon.Game.Components;
using System;
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

        public bool FindByResult(string substanceId, out SO_OutputRuleDefinition rule)
        {
            foreach (var r in rules)
            {
                if (r.ResultSubstance?.SubstanceId == substanceId)
                {
                    rule = r;
                    return true;
                }
            }

            rule = null;
            return false;
        }

        public bool TryGetMatch(
            string substanceId,
            TagComponent tags,
            FlowHistoryComponent history,
            Func<string, bool> isSubstanceEncountered,
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

                if (!MatchesUndiscoveredConditions(candidate, isSubstanceEncountered))
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

        private static bool MatchesUndiscoveredConditions(
            SO_OutputRuleDefinition rule,
            Func<string, bool> isSubstanceEncountered)
        {
            foreach (var substanceId in rule.RequiredUndiscoveredSubstanceIds)
            {
                if (string.IsNullOrWhiteSpace(substanceId))
                    continue;

                if (isSubstanceEncountered != null && isSubstanceEncountered(substanceId))
                    return false;
            }

            return true;
        }

        private static int CompareSpecificity(
            SO_OutputRuleDefinition left,
            SO_OutputRuleDefinition right)
        {
            var priorityCompare = right.Priority.CompareTo(left.Priority);
            if (priorityCompare != 0)
                return priorityCompare;

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

            return MatchesHistorySequence(requiredSequence, 0, history.Entries, 0);
        }

        private static bool MatchesHistorySequence(
            IReadOnlyList<string> requiredSequence,
            int requiredIndex,
            IReadOnlyList<string> historyEntries,
            int historyIndex)
        {
            requiredIndex = SkipEmptyRequiredEntries(requiredSequence, requiredIndex);

            if (requiredIndex >= requiredSequence.Count)
                return true;

            var requiredEntry = requiredSequence[requiredIndex];

            if (IsEdgeHistoryEntry(requiredEntry))
                return MatchesEdgeHistoryGroup(requiredSequence, requiredIndex, historyEntries, historyIndex);

            for (var i = historyIndex; i < historyEntries.Count; i++)
            {
                if (historyEntries[i] != requiredEntry)
                    continue;

                if (MatchesHistorySequence(requiredSequence, requiredIndex + 1, historyEntries, i + 1))
                    return true;
            }

            return false;
        }

        private static bool MatchesEdgeHistoryGroup(
            IReadOnlyList<string> requiredSequence,
            int requiredIndex,
            IReadOnlyList<string> historyEntries,
            int historyIndex)
        {
            var requiredEdges = new Dictionary<string, int>();
            var nextRequiredIndex = requiredIndex;

            while (nextRequiredIndex < requiredSequence.Count)
            {
                var requiredEntry = requiredSequence[nextRequiredIndex];

                if (string.IsNullOrWhiteSpace(requiredEntry))
                {
                    nextRequiredIndex++;
                    continue;
                }

                if (!IsEdgeHistoryEntry(requiredEntry))
                    break;

                AddRequiredEdge(requiredEdges, requiredEntry);
                nextRequiredIndex++;
            }

            nextRequiredIndex = SkipEmptyRequiredEntries(requiredSequence, nextRequiredIndex);

            if (nextRequiredIndex >= requiredSequence.Count)
                return ContainsRequiredEdges(historyEntries, historyIndex, historyEntries.Count, requiredEdges);

            var nextRequiredEntry = requiredSequence[nextRequiredIndex];

            for (var i = historyIndex; i < historyEntries.Count; i++)
            {
                if (historyEntries[i] != nextRequiredEntry)
                    continue;

                if (!ContainsRequiredEdges(historyEntries, historyIndex, i, requiredEdges))
                    continue;

                if (MatchesHistorySequence(requiredSequence, nextRequiredIndex + 1, historyEntries, i + 1))
                    return true;
            }

            return false;
        }

        private static bool ContainsRequiredEdges(
            IReadOnlyList<string> historyEntries,
            int startIndex,
            int endIndex,
            Dictionary<string, int> requiredEdges)
        {
            var remainingEdges = new Dictionary<string, int>(requiredEdges);

            for (var i = startIndex; i < endIndex; i++)
            {
                var historyEntry = historyEntries[i];

                if (!IsEdgeHistoryEntry(historyEntry))
                    continue;

                if (!remainingEdges.TryGetValue(historyEntry, out var remainingCount))
                    continue;

                if (remainingCount <= 1)
                    remainingEdges.Remove(historyEntry);
                else
                    remainingEdges[historyEntry] = remainingCount - 1;

                if (remainingEdges.Count <= 0)
                    return true;
            }

            return remainingEdges.Count <= 0;
        }

        private static int SkipEmptyRequiredEntries(
            IReadOnlyList<string> requiredSequence,
            int requiredIndex)
        {
            while (requiredIndex < requiredSequence.Count &&
                string.IsNullOrWhiteSpace(requiredSequence[requiredIndex]))
            {
                requiredIndex++;
            }

            return requiredIndex;
        }

        private static bool IsEdgeHistoryEntry(string entry)
        {
            return !string.IsNullOrWhiteSpace(entry) &&
                entry.StartsWith("edge:", System.StringComparison.Ordinal);
        }

        private static void AddRequiredEdge(
            Dictionary<string, int> requiredEdges,
            string edgeEntry)
        {
            if (requiredEdges.TryGetValue(edgeEntry, out var count))
                requiredEdges[edgeEntry] = count + 1;
            else
                requiredEdges.Add(edgeEntry, 1);
        }
    }
}
