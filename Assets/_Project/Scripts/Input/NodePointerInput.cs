using OneMoreSpoon.App.State;
using OneMoreSpoon.Game.Systems;
using OneMoreSpoon.View.Nodes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer;

namespace OneMoreSpoon.Input
{
    public sealed class NodePointerInput : MonoBehaviour
    {
        [SerializeField] private LayerMask nodeLayer;
        [SerializeField] private LayerMask edgeLayer;

        private SelectionState selectionState;
        private NodeMoveSystem nodeMoveSystem;
        private ClusterSeparationSystem clusterSeparationSystem;
        private EdgeConnectionState edgeConnectionState;
        private Camera mainCamera;
        private NodeView selectedView;
        private NodeView draggingView;
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
            mainCamera = Camera.main;
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
            if (IsPointerOverUI())
                return;

            if (RaycastMergeSlotHandle())
                return;

            var hitView = RaycastNodeView();
            if (hitView == null)
            {
                if (RaycastEdge())
                    return;

                ClearSelection();
                return;
            }

            Select(hitView);
            draggingView = hitView;
            draggingView.SetPressed(true);
            pointerToNodeOffset = (Vector2)hitView.transform.position - GetPointerWorldPosition();
        }

        private void DragPointer()
        {
            if (draggingView == null)
                return;

            Vector2 targetPosition = GetPointerWorldPosition() + pointerToNodeOffset;
            nodeMoveSystem.TryMoveNode(draggingView.EntityId, targetPosition);
        }

        private void EndPointer()
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

        private NodeView RaycastNodeView()
        {
            Vector2 worldPosition = GetPointerWorldPosition();
            var hit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity, nodeLayer);
            if (hit.collider == null)
                return null;

            return hit.collider.GetComponentInParent<NodeView>();
        }

        private Vector2 GetPointerWorldPosition()
        {
            EnsureCamera();
            Vector2 screenPosition = Pointer.current.position.ReadValue();
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
            return new Vector2(worldPosition.x, worldPosition.y);
        }

        private void EnsureCamera()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
        }

        private bool IsPointerOverUI()
        {
            return EventSystem.current != null
                && EventSystem.current.IsPointerOverGameObject();
        }

        private bool RaycastEdge()
        {
            Vector2 worldPosition = GetPointerWorldPosition();
            var hit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity, edgeLayer);
            return hit.collider != null;
        }

        private bool RaycastMergeSlotHandle()
        {
            Vector2 worldPosition = GetPointerWorldPosition();
            RaycastHit2D[] hits = Physics2D.RaycastAll(worldPosition, Vector2.zero);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.GetComponentInParent<MergeSlotHandle>() != null)
                    return true;
            }

            return false;
        }
    }
}
