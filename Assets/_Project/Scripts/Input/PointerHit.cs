using OneMoreSpoon.View.Edges;
using OneMoreSpoon.View.Nodes;
using OneMoreSpoon.View.Substances;

namespace OneMoreSpoon.Input
{
    public readonly struct PointerHit
    {
        public PointerHitType Type { get; }
        public NodeView NodeView { get; }
        public EdgeView EdgeView { get; }
        public SubstanceView SubstanceView { get; }

        private PointerHit(
            PointerHitType type,
            NodeView nodeView = null,
            EdgeView edgeView = null,
            SubstanceView substanceView = null)
        {
            Type = type;
            NodeView = nodeView;
            EdgeView = edgeView;
            SubstanceView = substanceView;
        }

        public static PointerHit None() => new(PointerHitType.None);
        public static PointerHit UI() => new(PointerHitType.UI);
        public static PointerHit MergeSlotHandle() => new(PointerHitType.MergeSlotHandle);
        public static PointerHit Node(NodeView nodeView) => new(PointerHitType.Node, nodeView);
        public static PointerHit Edge(EdgeView edgeView) => new(PointerHitType.Edge, edgeView: edgeView);
        public static PointerHit Substance(SubstanceView substanceView) => new(PointerHitType.Substance, substanceView: substanceView);
    }
}
