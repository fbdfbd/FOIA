using OneMoreSpoon.View.Nodes;

namespace OneMoreSpoon.View.Factories
{
    public sealed class NodeViewPrefabSet
    {
        public readonly NodeView Input;
        public readonly NodeView Output;
        public readonly NodeView Interact;
        public readonly NodeView Merge;

        public NodeViewPrefabSet(NodeView input, NodeView output, NodeView interact, NodeView merge)
        {
            Input = input;
            Output = output;
            Interact = interact;
            Merge = merge;
        }
    }
}