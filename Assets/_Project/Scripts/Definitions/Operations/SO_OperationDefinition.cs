using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    [CreateAssetMenu(
        fileName = "SO_OperationDefinition",
        menuName = "OneMoreSpoon/Definitions/Operation Definition")]
    public sealed class SO_OperationDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string operationId;
        [SerializeField] private string displayName;

        [Header("Classification")]
        [SerializeField] private OperationCategory category;
        [SerializeField] private List<string> baseTags = new();

        [Header("Execution")]
        [SerializeField] private float duration = 5f;

        [Header("Outputs")]
        [SerializeField] private List<string> baseOutputIds = new();
        [SerializeField] private List<string> outputTags = new();

        public string OperationId => operationId;
        public string DisplayName => displayName;
        public OperationCategory Category => category;
        public IReadOnlyList<string> BaseTags => baseTags;
        public float Duration => duration;
        public IReadOnlyList<string> BaseOutputIds => baseOutputIds;
        public IReadOnlyList<string> OutputTags => outputTags;
    }
}