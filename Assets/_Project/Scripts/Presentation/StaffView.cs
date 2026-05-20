using FOIA.Core;
using FOIA.Definitions;
using UnityEngine;

namespace FOIA.Presentation
{
    public sealed class StaffView : BoardInteractable
    {
        [SerializeField] private StaffDefinition definition;

        public StaffDefinition Definition => definition;
        public StaffId StaffId => definition != null ? definition.Id : default;
    }
}
