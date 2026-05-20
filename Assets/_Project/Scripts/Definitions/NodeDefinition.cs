using FOIA.Core;
using UnityEngine;

namespace FOIA.Definitions
{
    public enum NodeKind
    {
        Intake = 0,
        ResultBoard = 1,
        Agency = 2,
        FinalResult = 3
    }

    [CreateAssetMenu(menuName = "FOIA/Definitions/Node")]
    public sealed class NodeDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private NodeKind kind;

        public NodeId Id => new(id);
        public string DisplayName => displayName;
        public NodeKind Kind => kind;
    }
}
