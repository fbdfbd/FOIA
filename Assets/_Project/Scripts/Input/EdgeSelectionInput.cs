using OneMoreSpoon.App.State;
using OneMoreSpoon.Game.Systems;
using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Edges;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer;

namespace OneMoreSpoon.Input
{
    public sealed class EdgeSelectionInput : MonoBehaviour
    {
        [SerializeField] private LayerMask edgeLayer;

        private SelectionState selectionState;
        private SelectionVisualService selectionVisualService;
        private EdgeConnectionState edgeConnectionState;
        private EdgeDeleteSystem edgeDeleteSystem;
        private ViewRegistry viewRegistry;

        private Camera mainCamera;

        [Inject]
        public void Construct(
            SelectionState selectionState,
            SelectionVisualService selectionVisualService,
            EdgeConnectionState edgeConnectionState,
            EdgeDeleteSystem edgeDeleteSystem,
            ViewRegistry viewRegistry)
        {
            this.selectionState = selectionState;
            this.selectionVisualService = selectionVisualService;
            this.edgeConnectionState = edgeConnectionState;
            this.edgeDeleteSystem = edgeDeleteSystem;
            this.viewRegistry = viewRegistry;
        }

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Pointer.current == null)
                return;

            if (edgeConnectionState.IsConnecting)
                return;

            if (Pointer.current.press.wasPressedThisFrame)
                TrySelectEdge();

            if (Keyboard.current != null &&
                (Keyboard.current.deleteKey.wasPressedThisFrame ||
                 Keyboard.current.backspaceKey.wasPressedThisFrame))
            {
                TryDeleteSelectedEdge();
            }
        }

        private void TrySelectEdge()
        {
            if (IsPointerOverUI())
                return;

            var edgeView = RaycastEdgeView();

            if (edgeView == null)
                return;

            selectionVisualService.SelectEdge(edgeView);
        }

        private void TryDeleteSelectedEdge()
        {
            if (selectionState.SelectedType != SelectionTargetType.Edge)
                return;

            var edgeId = selectionState.SelectedEntityId;

            if (!edgeDeleteSystem.TryDeleteEdge(edgeId))
                return;

            if (viewRegistry.TryGetView(edgeId, out var view))
            {
                viewRegistry.Unregister(edgeId);
                Destroy(view.gameObject);
            }

            selectionState.Clear();
        }

        private EdgeView RaycastEdgeView()
        {
            Vector2 worldPosition = GetPointerWorldPosition();
            var hit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity, edgeLayer);

            if (hit.collider == null)
                return null;

            return hit.collider.GetComponentInParent<EdgeView>();
        }

        private Vector2 GetPointerWorldPosition()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            Vector2 screenPosition = Pointer.current.position.ReadValue();
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