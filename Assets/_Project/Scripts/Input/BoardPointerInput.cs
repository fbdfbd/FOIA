using FOIA.Presentation;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FOIA.Input
{
    public readonly struct BoardPointerEvent
    {
        public BoardPointerEvent(Vector2 screenPosition, Vector3 worldPosition, BoardInteractable target)
        {
            ScreenPosition = screenPosition;
            WorldPosition = worldPosition;
            Target = target;
        }

        public Vector2 ScreenPosition { get; }
        public Vector3 WorldPosition { get; }
        public BoardInteractable Target { get; }
    }

    public sealed class BoardPointerInput : MonoBehaviour
    {
        [SerializeField] private Camera boardCamera;
        [SerializeField] private InputActionReference pointAction;
        [SerializeField] private InputActionReference clickAction;
        [SerializeField] private LayerMask interactableLayerMask = ~0;
        [SerializeField] private float maxRayDistance = 100f;

        private readonly Subject<BoardPointerEvent> pointerDown = new();
        private readonly Subject<BoardPointerEvent> pointerUp = new();
        private readonly Subject<BoardPointerEvent> pointerMove = new();
        private Vector2 lastFallbackPoint;

        public Observable<BoardPointerEvent> PointerDown => pointerDown;
        public Observable<BoardPointerEvent> PointerUp => pointerUp;
        public Observable<BoardPointerEvent> PointerMove => pointerMove;

        private Camera ActiveCamera => boardCamera != null ? boardCamera : Camera.main;

        private void OnEnable()
        {
            if (pointAction != null)
            {
                pointAction.action.Enable();
                pointAction.action.performed += OnPoint;
            }

            if (clickAction != null)
            {
                clickAction.action.Enable();
                clickAction.action.started += OnClickStarted;
                clickAction.action.canceled += OnClickCanceled;
            }
        }

        private void OnDisable()
        {
            if (pointAction != null)
                pointAction.action.performed -= OnPoint;

            if (clickAction != null)
            {
                clickAction.action.started -= OnClickStarted;
                clickAction.action.canceled -= OnClickCanceled;
            }
        }

        private void OnDestroy()
        {
            pointerDown.Dispose();
            pointerUp.Dispose();
            pointerMove.Dispose();
        }

        private void Update()
        {
            if (pointAction != null || clickAction != null || Mouse.current == null)
                return;

            var point = Mouse.current.position.ReadValue();
            if (point != lastFallbackPoint)
            {
                lastFallbackPoint = point;
                pointerMove.OnNext(BuildEvent(point));
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
                pointerDown.OnNext(BuildEvent(point));

            if (Mouse.current.leftButton.wasReleasedThisFrame)
                pointerUp.OnNext(BuildEvent(point));
        }

        private void OnPoint(InputAction.CallbackContext context)
        {
            pointerMove.OnNext(BuildEvent(context.ReadValue<Vector2>()));
        }

        private void OnClickStarted(InputAction.CallbackContext context)
        {
            pointerDown.OnNext(BuildEvent(ReadPoint()));
        }

        private void OnClickCanceled(InputAction.CallbackContext context)
        {
            pointerUp.OnNext(BuildEvent(ReadPoint()));
        }

        private Vector2 ReadPoint()
        {
            return pointAction != null ? pointAction.action.ReadValue<Vector2>() : Vector2.zero;
        }

        private BoardPointerEvent BuildEvent(Vector2 screenPosition)
        {
            var camera = ActiveCamera;
            if (camera == null)
                return new BoardPointerEvent(screenPosition, Vector3.zero, null);

            var ray = camera.ScreenPointToRay(screenPosition);
            var target = default(BoardInteractable);
            var worldPosition = ray.origin;

            if (Physics.Raycast(ray, out var hit, maxRayDistance, interactableLayerMask))
            {
                worldPosition = hit.point;
                target = hit.collider.GetComponentInParent<BoardInteractable>();
            }
            else
            {
                var plane = new Plane(Vector3.back, Vector3.zero);
                if (plane.Raycast(ray, out var enter))
                    worldPosition = ray.GetPoint(enter);
            }

            return new BoardPointerEvent(screenPosition, worldPosition, target);
        }
    }
}
