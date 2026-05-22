using OneMoreSpoon.App.State;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.Game.Factories;
using OneMoreSpoon.View.Factories;
using OneMoreSpoon.View.Nodes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer;


namespace OneMoreSpoon.Input
{
    public sealed class EdgeConnectionInput : MonoBehaviour
    {
        [SerializeField] private LayerMask nodeLayer;
        [SerializeField] private LineRenderer previewLine; 
        [SerializeField] private SO_OperationDefinition defaultOperationDefinition;

        private EdgeConnectionState edgeConnectionState;
        private EdgeFactory edgeFactory;
        private EdgeViewFactory edgeViewFactory;

        private Camera mainCamera;
        private NodeView fromView;

        [Inject]
        public void Construct(
            EdgeConnectionState edgeConnectionState,
            EdgeFactory edgeFactory,
            EdgeViewFactory edgeViewFactory)
        {
            this.edgeConnectionState = edgeConnectionState;
            this.edgeFactory = edgeFactory;
            this.edgeViewFactory = edgeViewFactory;
        }

        private void Awake()
        {
            mainCamera = Camera.main;

            if (previewLine != null)
            {
                previewLine.positionCount = 2;
                previewLine.useWorldSpace = true;
                previewLine.enabled = false;
            }
        }

        private void Update()
        {
            if (Mouse.current == null)
                return;

            if (Mouse.current.rightButton.wasPressedThisFrame)
                BeginOrCancelConnection();

            if (edgeConnectionState.IsConnecting)
                UpdatePreviewLine();

            if (edgeConnectionState.IsConnecting && Mouse.current.leftButton.wasPressedThisFrame)
                CompleteConnection();
        }

        private void BeginOrCancelConnection()
        {
            if (IsPointerOverUI())
                return;

            var hitView = RaycastNodeView();

            if (hitView == null)
            {
                CancelConnection();
                return;
            }

            fromView = hitView;
            edgeConnectionState.Begin(hitView.EntityId);

            if (previewLine != null)
                previewLine.enabled = true;
        }

        private void CompleteConnection()
        {
            if (IsPointerOverUI())
                return;

            var toView = RaycastNodeView();

            if (toView == null)
            {
                CancelConnection();
                return;
            }

            if (edgeFactory.TryCreateEdge(
                edgeConnectionState.FromNodeId,
                toView.EntityId,
                defaultOperationDefinition,
                out var edgeId))
            {
                edgeViewFactory.Create(edgeId);
            }

            CancelConnection();
        }

        private void CancelConnection()
        {
            edgeConnectionState.Cancel();
            fromView = null;

            if (previewLine != null)
                previewLine.enabled = false;
        }

        private void UpdatePreviewLine()
        {
            if (previewLine == null || fromView == null)
                return;

            previewLine.SetPosition(0, fromView.transform.position);
            previewLine.SetPosition(1, GetPointerWorldPosition());
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
            if (mainCamera == null)
                mainCamera = Camera.main;

            Vector2 screenPosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
            return new Vector2(worldPosition.x, worldPosition.y);
        }

        private bool IsPointerOverUI()
        {
            return EventSystem.current != null
                && EventSystem.current.IsPointerOverGameObject();
        }
    }
}