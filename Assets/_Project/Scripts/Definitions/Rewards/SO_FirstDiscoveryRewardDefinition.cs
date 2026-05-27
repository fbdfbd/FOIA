using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    [CreateAssetMenu(
        fileName = "SO_FirstDiscoveryRewardDefinition",
        menuName = "OneMoreSpoon/Definitions/First Discovery Reward Definition")]
    public sealed class SO_FirstDiscoveryRewardDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string entryId;

        [Header("Trigger")]
        [SerializeField] private FirstDiscoveryRewardTriggerType triggerType;
        [SerializeField] private string triggerId;

        [Header("Reward")]
        [SerializeField] private FirstDiscoveryRewardType rewardType;
        [SerializeField] private string rewardId;
        [SerializeField] private int amount = 1;
        [SerializeField] private string rewardGroup;

        [Header("Conditions")]
        [SerializeField] private List<string> requiredUndiscoveredSubstanceIds = new();

        [Header("Memo")]
        [SerializeField] private string note;

        public string EntryId => entryId;
        public FirstDiscoveryRewardTriggerType TriggerType => triggerType;
        public string TriggerId => triggerId;
        public FirstDiscoveryRewardType RewardType => rewardType;
        public string RewardId => rewardId;
        public int Amount => Mathf.Max(1, amount);
        public string RewardGroup => rewardGroup;
        public IReadOnlyList<string> RequiredUndiscoveredSubstanceIds => requiredUndiscoveredSubstanceIds;
        public string Note => note;
    }
}
