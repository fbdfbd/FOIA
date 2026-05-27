using OneMoreSpoon.App.Encyclopedia;
using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.Game.Systems;
using UnityEngine;

namespace OneMoreSpoon.App.Rewards
{
    public sealed class FirstDiscoveryRewardService
    {
        private static readonly Vector2 SubstanceRewardOffset = new(0f, -0.8f);
        private static readonly Vector2 SubstanceRewardSpacing = new(0.55f, 0f);

        private readonly GameWorld world;
        private readonly FirstDiscoveryRewardRegistry rewardRegistry;
        private readonly FirstDiscoveryRewardState rewardState;
        private readonly DiscoveryService discoveryService;
        private readonly SubstanceDefinitionRegistry substanceDefinitions;
        private readonly NodeDefinitionRegistry nodeDefinitions;
        private readonly NodeInventoryState nodeInventory;
        private readonly PlayAreaBoundsSystem playAreaBounds;

        public FirstDiscoveryRewardService(
            GameWorld world,
            FirstDiscoveryRewardRegistry rewardRegistry,
            FirstDiscoveryRewardState rewardState,
            DiscoveryService discoveryService,
            SubstanceDefinitionRegistry substanceDefinitions,
            NodeDefinitionRegistry nodeDefinitions,
            NodeInventoryState nodeInventory,
            PlayAreaBoundsSystem playAreaBounds)
        {
            this.world = world;
            this.rewardRegistry = rewardRegistry;
            this.rewardState = rewardState;
            this.discoveryService = discoveryService;
            this.substanceDefinitions = substanceDefinitions;
            this.nodeDefinitions = nodeDefinitions;
            this.nodeInventory = nodeInventory;
            this.playAreaBounds = playAreaBounds;
        }

        public void GrantForSubstance(string substanceId, Vector2 basePosition)
        {
            Grant(FirstDiscoveryRewardTriggerType.Substance, substanceId, basePosition);
        }

        public void GrantForOutputRule(string ruleId, Vector2 basePosition)
        {
            Grant(FirstDiscoveryRewardTriggerType.OutputRule, ruleId, basePosition);
        }

        public void GrantForMergeRecipe(string recipeId, Vector2 basePosition)
        {
            Grant(FirstDiscoveryRewardTriggerType.MergeRecipe, recipeId, basePosition);
        }

        private void Grant(
            FirstDiscoveryRewardTriggerType triggerType,
            string triggerId,
            Vector2 basePosition)
        {
            var triggerKey = FirstDiscoveryRewardRegistry.BuildTriggerKey(triggerType, triggerId);
            if (!rewardState.TryCompleteTrigger(triggerKey))
                return;

            var rewards = rewardRegistry.GetRewards(triggerType, triggerId);
            var substanceRewardIndex = 0;

            foreach (var reward in rewards)
            {
                if (!CanGrant(reward))
                    continue;

                if (!rewardState.CanClaimGroup(reward.RewardGroup))
                    continue;

                if (reward.RewardType == FirstDiscoveryRewardType.Substance)
                {
                    if (CreateSubstanceReward(reward, basePosition, substanceRewardIndex))
                    {
                        rewardState.TryClaimGroup(reward.RewardGroup);
                        substanceRewardIndex++;
                    }

                    continue;
                }

                if (reward.RewardType == FirstDiscoveryRewardType.Node)
                {
                    if (AddNodeReward(reward))
                        rewardState.TryClaimGroup(reward.RewardGroup);
                }
            }
        }

        private bool CanGrant(SO_FirstDiscoveryRewardDefinition reward)
        {
            foreach (var substanceId in reward.RequiredUndiscoveredSubstanceIds)
            {
                if (string.IsNullOrWhiteSpace(substanceId))
                    continue;

                if (discoveryService.IsEncountered(substanceId))
                    return false;
            }

            return true;
        }

        private bool CreateSubstanceReward(
            SO_FirstDiscoveryRewardDefinition reward,
            Vector2 basePosition,
            int index)
        {
            if (!substanceDefinitions.TryGet(reward.RewardId, out var substance))
            {
                Debug.LogWarning($"[FirstDiscoveryReward] Substance reward skipped. Missing substance id={reward.RewardId}");
                return false;
            }

            var position = playAreaBounds.Clamp(basePosition + SubstanceRewardOffset + SubstanceRewardSpacing * index);
            var stackId = world.CreateSubstanceStack(substance.SubstanceId, reward.Amount, false, position);

            Debug.Log($"[FirstDiscoveryReward] Substance granted id={substance.SubstanceId} amount={reward.Amount} stack={stackId}");
            return true;
        }

        private bool AddNodeReward(SO_FirstDiscoveryRewardDefinition reward)
        {
            if (!nodeDefinitions.TryGet(reward.RewardId, out var node))
            {
                Debug.LogWarning($"[FirstDiscoveryReward] Node reward skipped. Missing node id={reward.RewardId}");
                return false;
            }

            nodeInventory.Add(node.DefinitionId, reward.Amount);
            Debug.Log($"[FirstDiscoveryReward] Node granted id={node.DefinitionId} amount={reward.Amount}");
            return true;
        }
    }
}
