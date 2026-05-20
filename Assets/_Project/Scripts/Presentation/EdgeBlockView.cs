using FOIA.Core;
using FOIA.Definitions;
using UnityEngine;

namespace FOIA.Presentation
{
    public sealed class EdgeBlockView : BoardInteractable
    {
        [SerializeField] private EdgeBlockDefinition definition;

        public EdgeBlockDefinition Definition => definition;
        public EdgeBlockId BlockId => definition != null ? definition.Id : default;
    }
}
