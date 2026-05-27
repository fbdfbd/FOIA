using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    public sealed class FirstDiscoveryRewardRegistry
    {
        private readonly Dictionary<string, List<SO_FirstDiscoveryRewardDefinition>> rewardsByTrigger = new();

        public FirstDiscoveryRewardRegistry(IEnumerable<SO_FirstDiscoveryRewardDefinition> rewards)
        {
            if (rewards == null)
                return;

            foreach (var reward in rewards)
            {
                if (reward == null)
                    continue;

                if (string.IsNullOrWhiteSpace(reward.TriggerId))
                {
                    Debug.LogWarning($"First discovery reward ignored because trigger id is empty: {reward.name}");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(reward.RewardId))
                {
                    Debug.LogWarning($"First discovery reward ignored because reward id is empty: {reward.name}");
                    continue;
                }

                var key = BuildTriggerKey(reward.TriggerType, reward.TriggerId);
                if (!rewardsByTrigger.TryGetValue(key, out var list))
                {
                    list = new List<SO_FirstDiscoveryRewardDefinition>();
                    rewardsByTrigger.Add(key, list);
                }

                list.Add(reward);
            }
        }

        public IReadOnlyList<SO_FirstDiscoveryRewardDefinition> GetRewards(
            FirstDiscoveryRewardTriggerType triggerType,
            string triggerId)
        {
            if (string.IsNullOrWhiteSpace(triggerId))
                return System.Array.Empty<SO_FirstDiscoveryRewardDefinition>();

            var key = BuildTriggerKey(triggerType, triggerId);
            return rewardsByTrigger.TryGetValue(key, out var rewards)
                ? rewards
                : System.Array.Empty<SO_FirstDiscoveryRewardDefinition>();
        }

        public static string BuildTriggerKey(
            FirstDiscoveryRewardTriggerType triggerType,
            string triggerId)
        {
            return $"{triggerType}:{triggerId}";
        }
    }
}
