using System;

namespace FOIA.Graph.Runtime
{
    [Flags]
    public enum NodeRuntimeTag
    {
        None = 0,
        Connected = 1 << 0,
    }

    [Flags]
    public enum EdgeRuntimeTag
    {
        None = 0,
        Connected = 1 << 0,
        Selected = 1 << 1,
    }

    public sealed class EdgeRuntimeData
    {
        public EdgeRuntimeData(
            string edgeId,
            string fromNodeId,
            string toNodeId,
            EdgeDirection direction = EdgeDirection.Forward)
        {
            EdgeId = edgeId;
            FromNodeId = fromNodeId;
            ToNodeId = toNodeId;
            Direction = direction;
            Tags = EdgeRuntimeTag.Connected;
        }

        public string EdgeId { get; }
        public string FromNodeId { get; }
        public string ToNodeId { get; }
        public EdgeDirection Direction { get; private set; }
        public EdgeRuntimeTag Tags { get; private set; }

        public bool Connects(string firstNodeId, string secondNodeId)
        {
            return FromNodeId == firstNodeId && ToNodeId == secondNodeId
                || FromNodeId == secondNodeId && ToNodeId == firstNodeId;
        }

        public bool ConnectsDirected(string fromNodeId, string toNodeId)
        {
            return FromNodeId == fromNodeId && ToNodeId == toNodeId;
        }

        public bool MatchesDuplicate(string fromNodeId, string toNodeId, EdgeDuplicatePolicy duplicatePolicy)
        {
            return duplicatePolicy switch
            {
                EdgeDuplicatePolicy.AllowParallel => false,
                EdgeDuplicatePolicy.SameDirectionOnly => ConnectsDirected(fromNodeId, toNodeId),
                _ => Connects(fromNodeId, toNodeId),
            };
        }

        public void SetDirection(EdgeDirection direction)
        {
            Direction = direction;
        }

        public void SetSelected(bool isSelected)
        {
            if (isSelected)
            {
                Tags |= EdgeRuntimeTag.Selected;
                return;
            }

            Tags &= ~EdgeRuntimeTag.Selected;
        }
    }
}
