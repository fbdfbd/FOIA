using OneMoreSpoon.App.State;
using OneMoreSpoon.Game.Systems;
using OneMoreSpoon.View.Common;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace OneMoreSpoon.Input
{
    [RequireComponent(typeof(PointerHitResolver))]
    public sealed class EdgeSelectionInput : MonoBehaviour
    {
        [SerializeField] private PointerHitResolver pointerHitResolver;

        private SelectionState selectionState;
        private SelectionVisualService selectionVisualService;
        private EdgeConnectionState edgeConnectionState;
        private EdgeDeleteSystem edgeDeleteSystem;
        private ViewRegistry viewRegistry;

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
            if (pointerHitResolver == null)
                pointerHitResolver = GetComponent<PointerHitResolver>();
        }

        private void Update()
        {
            if (Pointer.current == null)
                return;

            if (edgeConnectionState.IsConnecting)
                return;

            if (Pointer.current.press.wasPressedThisFrame)
                TrySelectEdge();

            if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
                TryDeleteSelectedEdgeUnderPointer();
        }

        private void TrySelectEdge()
        {
            PointerHit hit = pointerHitResolver.Resolve();

            if (hit.Type != PointerHitType.Edge)
                return;

            selectionVisualService.SelectEdge(hit.EdgeView);
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

        private void TryDeleteSelectedEdgeUnderPointer()
        {
            if (selectionState.SelectedType != SelectionTargetType.Edge)
                return;

            PointerHit hit = pointerHitResolver.Resolve();

            if (hit.Type != PointerHitType.Edge ||
                hit.EdgeView.EntityId != selectionState.SelectedEntityId)
                return;

            TryDeleteSelectedEdge();
        }
    }
}
