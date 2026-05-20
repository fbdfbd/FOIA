using FOIA.Flow.Runtime;
using FOIA.Graph.Runtime;
using UnityEngine;

namespace FOIA.Flow.Input
{
    public sealed class FlowDebugHotkeys : MonoBehaviour
    {
        [SerializeField] private FlowSystem flowSystem;
        [SerializeField] private KeyCode createComplaintKey = KeyCode.Alpha1;
        [SerializeField] private KeyCode putOnSelectedNodeKey = KeyCode.Alpha2;
        [SerializeField] private KeyCode moveThroughSelectedEdgeKey = KeyCode.Alpha3;
        [SerializeField] private KeyCode sendToOutputKey = KeyCode.Alpha4;

        private void Awake()
        {
            if (flowSystem == null)
            {
                flowSystem = GraphSceneLookup.FindFirst<FlowSystem>();
            }
        }

        private void Update()
        {
            if (flowSystem == null)
            {
                return;
            }

            if (UnityEngine.Input.GetKeyDown(createComplaintKey))
            {
                flowSystem.CreateDefaultComplaint();
            }

            if (UnityEngine.Input.GetKeyDown(putOnSelectedNodeKey))
            {
                flowSystem.PutSelectedItemOnSelectedNode();
            }

            if (UnityEngine.Input.GetKeyDown(moveThroughSelectedEdgeKey))
            {
                flowSystem.MoveSelectedItemThroughSelectedEdge();
            }

            if (UnityEngine.Input.GetKeyDown(sendToOutputKey))
            {
                flowSystem.SendSelectedItemToOutput();
            }
        }
    }
}
