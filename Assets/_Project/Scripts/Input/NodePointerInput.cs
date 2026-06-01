using OneMoreSpoon.App.State;
using OneMoreSpoon.Game.Systems;
using OneMoreSpoon.View.Nodes;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace OneMoreSpoon.Input
{
    [RequireComponent(typeof(PointerHitResolver))]
    public sealed class NodePointerInput : MonoBehaviour
    {
        [SerializeField] private PointerHitResolver pointerHitResolver;

        private SelectionState selectionState;
        private NodeMoveSystem nodeMoveSystem;
        private ClusterSeparationSystem clusterSeparationSystem;
        private EdgeConnectionState edgeConnectionState;
        private NodeView selectedView;
        private NodeView draggingView;
        private Vector2 dragStartPosition;
        private Vector2 pointerToNodeOffset;
        private SelectionVisualService selectionVisualService;

        [Inject]
        public void Construct(
            SelectionState selectionState,
            SelectionVisualService selectionVisualService,
            NodeMoveSystem nodeMoveSystem,
            ClusterSeparationSystem clusterSeparationSystem,
            EdgeConnectionState edgeConnectionState)
        {
            this.selectionState = selectionState;
            this.selectionVisualService = selectionVisualService;
            this.nodeMoveSystem = nodeMoveSystem;
            this.clusterSeparationSystem = clusterSeparationSystem;
            this.edgeConnectionState = edgeConnectionState;
        }

        private void Awake()
        {
            if (pointerHitResolver == null)
                pointerHitResolver = GetComponent<PointerHitResolver>();
        }

        private void Update()
        {
            if (edgeConnectionState != null && edgeConnectionState.IsConnecting)
                return;

            if (Pointer.current == null)
                return;

            if (Pointer.current.press.wasPressedThisFrame)
                BeginPointer();

            if (Pointer.current.press.isPressed)
                DragPointer();

            if (Pointer.current.press.wasReleasedThisFrame)
                EndPointer();
        }

        private void BeginPointer()
        {
            PointerHit hit = pointerHitResolver.Resolve();

            if (hit.Type != PointerHitType.Node)
            {
                if (hit.Type != PointerHitType.None)
                    return;

                ClearSelection();
                return;
            }

            Select(hit.NodeView);
            draggingView = hit.NodeView;
            draggingView.SetPressed(true);
            dragStartPosition = hit.NodeView.transform.position;
            pointerToNodeOffset = (Vector2)hit.NodeView.transform.position - pointerHitResolver.GetWorldPosition();
        }

        private void DragPointer()
        {
            if (draggingView == null)
                return;

            Vector2 targetPosition = pointerHitResolver.GetWorldPosition() + pointerToNodeOffset;
            nodeMoveSystem.TryMoveNode(draggingView.EntityId, targetPosition);
        }

        private void EndPointer()
        {
            if (draggingView == null)
                return;

            if (pointerHitResolver.IsPointerOverTrashCan())
                nodeMoveSystem.TryMoveNode(draggingView.EntityId, dragStartPosition);

            ReleaseDraggingNode();
        }

        private void ReleaseDraggingNode()
        {
            if (draggingView != null)
            {
                draggingView.SetPressed(false);
                clusterSeparationSystem.RelaxAround(draggingView.EntityId);
            }

            draggingView = null;
        }

        private void Select(NodeView nodeView)
        {
            selectedView = nodeView;
            selectionVisualService.SelectNode(nodeView);
        }

        private void ClearSelection()
        {
            if (draggingView != null)
                draggingView.SetPressed(false);

            selectedView = null;
            draggingView = null;
            selectionVisualService.Clear();
        }
    }
}
