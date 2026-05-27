using System.Collections.Generic;

namespace OneMoreSpoon.App.Rewards
{
    public sealed class FirstDiscoveryRewardState
    {
        private readonly HashSet<string> completedTriggers = new();
        private readonly HashSet<string> claimedRewardGroups = new();

        public bool HasCompletedTrigger(string triggerKey)
        {
            return completedTriggers.Contains(triggerKey);
        }

        public bool TryCompleteTrigger(string triggerKey)
        {
            if (string.IsNullOrWhiteSpace(triggerKey))
                return false;

            return completedTriggers.Add(triggerKey);
        }

        public bool CanClaimGroup(string rewardGroup)
        {
            return string.IsNullOrWhiteSpace(rewardGroup)
                || !claimedRewardGroups.Contains(rewardGroup);
        }

        public bool TryClaimGroup(string rewardGroup)
        {
            if (string.IsNullOrWhiteSpace(rewardGroup))
                return true;

            return claimedRewardGroups.Add(rewardGroup);
        }
    }
}
