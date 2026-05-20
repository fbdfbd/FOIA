using System;

namespace FOIA.Flow.Definitions
{
    [Serializable]
    public sealed class FlowTagEffect
    {
        public FlowTagOperation Operation = FlowTagOperation.Add;
        public string Tag;
    }
}
