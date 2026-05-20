using System;
using FOIA.Graph.Input;
using UnityEngine;

namespace FOIA.Graph.Runtime
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class NodeEntity : MonoBehaviour
    {
        [SerializeField] private GraphRuntimeStore graphStore;
        [SerializeField] private string nodeId;
        [SerializeField] private RectTransform connectionAnchor;

        private NodeRuntimeTag tags;

        public string NodeId => nodeId;
        public RectTransform ConnectionAnchor => connectionAnchor != null ? connectionAnchor : (RectTransform)transform;
        public NodeRuntimeTag Tags => tags;

        private void Awake()
        {
            if (string.IsNullOrWhiteSpace(nodeId))
            {
                nodeId = Guid.NewGuid().ToString("N");
            }

            if (graphStore == null)
            {
                graphStore = GraphSceneLookup.FindFirst<GraphRuntimeStore>();
            }
        }

        private void OnEnable()
        {
            EnsureSelectionInput();
            graphStore?.RegisterNode(this);
        }

        private void OnDisable()
        {
            graphStore?.UnregisterNode(this);
        }

        public void SetConnected(bool isConnected)
        {
            if (isConnected)
            {
                tags |= NodeRuntimeTag.Connected;
                return;
            }

            tags &= ~NodeRuntimeTag.Connected;
        }

        private void EnsureSelectionInput()
        {
            if (!TryGetComponent(out NodeSelectionInput selectionInput))
            {
                selectionInput = gameObject.AddComponent<NodeSelectionInput>();
            }

            selectionInput.Initialize(graphStore, this);
        }
    }
}
