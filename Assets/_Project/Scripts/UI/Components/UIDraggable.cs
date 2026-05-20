using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FOIA.UI.Components
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class UIDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private bool interactable = true;
        [SerializeField] private RectTransform dragRoot;
        [SerializeField] private PointerDragEvent dragStarted = new();
        [SerializeField] private PointerDragEvent dragged = new();
        [SerializeField] private PointerDragEvent dragEnded = new();

        private RectTransform target;
        private RectTransform parentRect;
        private Vector2 pointerOffset;
        private bool isDragging;

        public bool Interactable
        {
            get => interactable;
            set => interactable = value;
        }

        public RectTransform DragRoot
        {
            get => dragRoot;
            set
            {
                dragRoot = value;
                ResolveDragTarget();
            }
        }

        public UnityEvent<PointerEventData> DragStarted => dragStarted;
        public UnityEvent<PointerEventData> Dragged => dragged;
        public UnityEvent<PointerEventData> DragEnded => dragEnded;

        private void Awake()
        {
            ResolveDragTarget();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            ResolveDragTarget();

            if (!interactable || parentRect == null || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (!TryGetPointerLocalPosition(eventData, out Vector2 localPointerPosition))
            {
                return;
            }

            pointerOffset = target.anchoredPosition - localPointerPosition;
            isDragging = true;
            dragStarted.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging || !interactable || parentRect == null)
            {
                return;
            }

            if (!TryGetPointerLocalPosition(eventData, out Vector2 localPointerPosition))
            {
                return;
            }

            target.anchoredPosition = localPointerPosition + pointerOffset;
            dragged.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging)
            {
                return;
            }

            isDragging = false;
            dragEnded.Invoke(eventData);
        }

        private void ResolveDragTarget()
        {
            target = dragRoot != null ? dragRoot : (RectTransform)transform;
            parentRect = target.parent as RectTransform;
        }

        private bool TryGetPointerLocalPosition(PointerEventData eventData, out Vector2 localPosition)
        {
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                eventData.pressEventCamera,
                out localPosition);
        }

        [System.Serializable]
        private sealed class PointerDragEvent : UnityEvent<PointerEventData>
        {
        }
    }
}
