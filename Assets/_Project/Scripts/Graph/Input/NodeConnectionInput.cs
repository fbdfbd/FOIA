using FOIA.Graph.Runtime;
using FOIA.UI.Components;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FOIA.Graph.Input
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NodeEntity))]
    [RequireComponent(typeof(UIRightClickable))]
    public sealed class NodeConnectionInput : MonoBehaviour
    {
        [SerializeField] private ConnectionSystem connectionSystem;

        private NodeEntity node;
        private UIRightClickable rightClickable;

        private void Awake()
        {
            node = GetComponent<NodeEntity>();
            rightClickable = GetComponent<UIRightClickable>();

            if (connectionSystem == null)
            {
                connectionSystem = GraphSceneLookup.FindFirst<ConnectionSystem>();
            }
        }

        private void OnEnable()
        {
            rightClickable.RightClicked.AddListener(OnRightClicked);
        }

        private void OnDisable()
        {
            rightClickable.RightClicked.RemoveListener(OnRightClicked);
        }

        private void OnRightClicked(PointerEventData eventData)
        {
            connectionSystem?.HandleNodeRightClicked(node);
        }
    }
}
