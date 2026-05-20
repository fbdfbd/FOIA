using FOIA.Core;
using FOIA.Definitions;
using UnityEngine;

namespace FOIA.Presentation
{
    public sealed class EdgeSlotView : BoardInteractable
    {
        [SerializeField] private EdgeDefinition definition;
        [SerializeField] private Transform attachPoint;

        public EdgeDefinition Definition => definition;
        public EdgeId EdgeId => definition != null ? definition.Id : default;
        public Transform AttachPoint => attachPoint != null ? attachPoint : transform;
    }
}
