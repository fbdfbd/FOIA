using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    [CreateAssetMenu(
        fileName = "SO_NodeInspectDefinition",
        menuName = "OneMoreSpoon/Definitions/Node Inspect Definition")]
    public sealed class SO_NodeInspectDefinition : ScriptableObject
    {
        [Header("Key")]
        [SerializeField] private string targetDefinitionId;

        [Header("Display")]
        [SerializeField] private string description;

        public string TargetDefinitionId => targetDefinitionId;
        public string Description => description;
    }
}
