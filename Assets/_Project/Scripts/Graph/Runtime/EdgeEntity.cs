using UnityEngine;

namespace FOIA.Graph.Runtime
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class EdgeEntity : MonoBehaviour
    {
        public EdgeRuntimeData Data { get; private set; }
        public NodeEntity FromNode { get; private set; }
        public NodeEntity ToNode { get; private set; }

        public string EdgeId => Data != null ? Data.EdgeId : string.Empty;

        public void Initialize(EdgeRuntimeData data, NodeEntity fromNode, NodeEntity toNode)
        {
            Data = data;
            FromNode = fromNode;
            ToNode = toNode;
            name = $"Edge_{fromNode.name}_{toNode.name}";
        }
    }
}
