using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    [CreateAssetMenu(
        fileName = "SO_NodeDefinition",
        menuName = "OneMoreSpoon/Definitions/Node Definition")]
    public sealed class SO_NodeDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string definitionId;
        [SerializeField] private string displayName;

        [Header("Classification")]
        [SerializeField] private ProcessLayer processLayer;
        [SerializeField] private NodeCategory category;
        [SerializeField] private List<string> baseTags = new();

        public string DefinitionId => definitionId;
        public string DisplayName => displayName;
        public ProcessLayer ProcessLayer => processLayer;
        public NodeCategory Category => category;
        public IReadOnlyList<string> BaseTags => baseTags;
    }
}