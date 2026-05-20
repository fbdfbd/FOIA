using FOIA.Core;
using FOIA.Definitions;
using UnityEngine;

namespace FOIA.Presentation
{
    public sealed class AgencyView : BoardInteractable
    {
        [SerializeField] private AgencyDefinition definition;

        public AgencyDefinition Definition => definition;
        public AgencyId AgencyId => definition != null ? definition.Id : default;
    }
}
