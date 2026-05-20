using FOIA.Flow.Definitions;
using UnityEngine;

namespace FOIA.Flow.Runtime
{
    public sealed class AgencyRuntime
    {
        public AgencyRuntime(AgencyDefinition definition)
        {
            Definition = definition;
            Relationship = definition != null ? definition.StartRelationship : 50;
        }

        public AgencyDefinition Definition { get; }
        public int Relationship { get; private set; }

        public void AddRelationship(int value)
        {
            Relationship = Mathf.Clamp(Relationship + value, 0, 100);
        }
    }
}
