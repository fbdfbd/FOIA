using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    [CreateAssetMenu(
        fileName = "SO_SubstanceInspectDefinition",
        menuName = "OneMoreSpoon/Definitions/Substance Inspect Definition")]
    public sealed class SO_SubstanceInspectDefinition : ScriptableObject
    {
        [Header("Key")]
        [SerializeField] private string targetSubstanceId;

        [Header("Display")]
        [SerializeField] private string description;

        public string TargetSubstanceId => targetSubstanceId;
        public string Description => description;
    }
}
