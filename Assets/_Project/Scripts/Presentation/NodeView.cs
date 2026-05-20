using FOIA.Core;
using FOIA.Definitions;
using UnityEngine;

namespace FOIA.Presentation
{
    public sealed class NodeView : BoardInteractable
    {
        [SerializeField] private NodeDefinition definition;

        public NodeDefinition Definition => definition;
        public NodeId NodeId => definition != null ? definition.Id : default;
    }
}
