using OneMoreSpoon.App.State;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer;

namespace OneMoreSpoon.Input
{
    public sealed class CameraViewportInput : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private LayerMask blockingLayer;
        [SerializeField, Min(0.1f)] private float zoomInSize = 3f;
        [SerializeField, Min(0.1f)] private float zoomOutSize = 8f;
        [SerializeField, Min(0.0001f)] private float zoomStep = 0.005f;

        private float targetZoomSize;
        private bool isZoomInitialized;
        private bool isPanning;
        private bool isLeftButtonPanning;
        private bool nextPanUsesLeftButton;
        private Vector2 panAnchorWorldPosition;
        private EdgeConnectionState edgeConnectionState;

        [Inject]
        public void Construct(EdgeConnectionState edgeConnectionState)
        {
            this.edgeConnectionState = edgeConnectionState;
        }

        private void Awake()
        {
            EnsureCamera();

            InitializeZoomSize();
        }

        private void Update()
        {
            if (Mouse.current == null)
                return;

            EnsureCamera();

            if (targetCamera == null)
                return;

            HandleZoom();
            HandlePan();
        }

        private void HandleZoom()
        {
            if (IsPointerOverUI())
                return;

            float scrollY = Mouse.current.scroll.ReadValue().y;

            if (scrollY != 0f)
                targetZoomSize = Mathf.Clamp(
                    targetZoomSize - scrollY * zoomStep,
                    zoomInSize,
                    zoomOutSize);

            targetCamera.orthographicSize = Mathf.Lerp(
                targetCamera.orthographicSize,
                targetZoomSize,
                1f - Mathf.Exp(-18f * Time.deltaTime));
        }

        private void HandlePan()
        {
            if (IsPanPressedThisFrame())
                BeginPan();

            if (isPanning && IsActivePanButtonPressed())
                Pan();

            if (isPanning && IsActivePanButtonReleasedThisFrame())
            {
                isPanning = false;
                isLeftButtonPanning = false;
            }
        }

        private void BeginPan()
        {
            if (IsPointerOverUI())
                return;

            if (RaycastBlockingObject())
                return;

            isPanning = true;
            isLeftButtonPanning = nextPanUsesLeftButton;
            panAnchorWorldPosition = GetPointerWorldPosition();
        }

        private bool IsPanPressedThisFrame()
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                nextPanUsesLeftButton = false;
                return true;
            }

            if (IsEdgeConnecting())
                return false;

            nextPanUsesLeftButton = true;
            return Mouse.current.leftButton.wasPressedThisFrame;
        }

        private bool IsActivePanButtonPressed()
        {
            return isLeftButtonPanning
                ? Mouse.current.leftButton.isPressed
                : Mouse.current.rightButton.isPressed;
        }

        private bool IsActivePanButtonReleasedThisFrame()
        {
            return isLeftButtonPanning
                ? Mouse.current.leftButton.wasReleasedThisFrame
                : Mouse.current.rightButton.wasReleasedThisFrame;
        }

        private bool IsEdgeConnecting()
        {
            return edgeConnectionState != null && edgeConnectionState.IsConnecting;
        }

        private void Pan()
        {
            Vector2 currentWorldPosition = GetPointerWorldPosition();
            Vector2 delta = panAnchorWorldPosition - currentWorldPosition;

            targetCamera.transform.position += new Vector3(delta.x, delta.y, 0f);
        }

        private Vector2 GetPointerWorldPosition()
        {
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = targetCamera.ScreenToWorldPoint(screenPosition);
            return new Vector2(worldPosition.x, worldPosition.y);
        }

        private void EnsureCamera()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;

            InitializeZoomSize();
        }

        private void InitializeZoomSize()
        {
            if (isZoomInitialized || targetCamera == null)
                return;

            targetZoomSize = targetCamera.orthographicSize;
            isZoomInitialized = true;
        }

        private bool RaycastBlockingObject()
        {
            Vector2 worldPosition = GetPointerWorldPosition();
            var hit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity, blockingLayer);
            return hit.collider != null;
        }

        private bool IsPointerOverUI()
        {
            return EventSystem.current != null
                && EventSystem.current.IsPointerOverGameObject();
        }
    }
}
