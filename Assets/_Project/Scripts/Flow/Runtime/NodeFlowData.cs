using FOIA.Flow.Definitions;
using UnityEngine;

namespace FOIA.Flow.Runtime
{
    [DisallowMultipleComponent]
    public sealed class NodeFlowData : MonoBehaviour
    {
        [SerializeField] private NodeFlowRole role;
        [SerializeField] private AgencyDefinition agency;

        public NodeFlowRole Role
        {
            get => role;
            set => role = value;
        }

        public AgencyDefinition Agency
        {
            get => agency;
            set => agency = value;
        }
    }
}
