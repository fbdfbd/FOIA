using OneMoreSpoon.App.Messaging;
using UnityEngine;

namespace OneMoreSpoon.View.UI
{
    public sealed class NodeFoundPanelActivator : MonoBehaviour
    {
        [SerializeField] private string targetNodeId = "node_place_basement";
        [SerializeField] private GameObject panel;
        [SerializeField] private bool hideOnAwake = true;
        [SerializeField] private bool activateOnce = true;

        private bool activated;

        private void Awake()
        {
            if (hideOnAwake && panel != null)
                panel.SetActive(false);
        }

        private void OnEnable()
        {
            NodeDiscoveryEvents.Discovered += OnNodeDiscovered;
        }

        private void OnDisable()
        {
            NodeDiscoveryEvents.Discovered -= OnNodeDiscovered;
        }

        private void OnNodeDiscovered(string nodeId)
        {
            if (activateOnce && activated)
                return;

            if (nodeId != targetNodeId)
                return;

            activated = true;

            if (panel != null)
                panel.SetActive(true);
        }
    }
}
