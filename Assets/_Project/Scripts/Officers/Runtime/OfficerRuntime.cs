using System.Collections.Generic;
using FOIA.Core.Tags;
using FOIA.Officers.Definitions;

namespace FOIA.Officers.Runtime
{
    public sealed class OfficerRuntime
    {
        public string RuntimeId { get; }
        public SO_OfficerDefinition Definition { get; }

        public int Stress { get; private set; }
        public OfficerStatus Status { get; private set; }
        public List<GameTag> DynamicTags { get; } = new();

        public OfficerRuntime(string runtimeId, SO_OfficerDefinition definition)
        {
            RuntimeId = runtimeId;
            Definition = definition;
            Status = OfficerStatus.Available;
        }

        public void SetStress(int stress)
        {
            Stress = stress;
        }

        public void SetStatus(OfficerStatus status)
        {
            Status = status;
        }
    }
}
