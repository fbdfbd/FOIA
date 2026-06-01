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
        [SerializeField] private Sprite image;

        [Header("Classification")]
        [SerializeField] private int processLayer;
        [SerializeField] private NodeCategory category;
        [SerializeField] private List<string> baseTags = new();

        [Header("Flow Effect")]
        [SerializeField] private List<string> addedFlowTags = new();

        public string DefinitionId => definitionId;
        public string DisplayName => displayName;
        public Sprite Image => image;
        public int ProcessLayer => processLayer;
        public NodeCategory Category => category;
        public IReadOnlyList<string> BaseTags => baseTags;
        public IReadOnlyList<string> AddedFlowTags => addedFlowTags;
    }
}
