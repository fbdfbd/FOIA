using UnityEngine;
using UnityEngine.EventSystems;

namespace FOIA.Presentation.Views.Cards
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class DraggableCardViewBase : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, ICardDragPayloadProvider
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        private RectTransform _rectTransform;
        private Transform _originalParent;
        private int _originalSiblingIndex;
        private Vector2 _originalAnchoredPosition;
        private RectTransform _dragRoot;
        private Camera _eventCamera;
        private bool _dropAccepted;

        public CardDragPayload Payload { get; private set; }

        protected abstract CardDragPayload CreateDragPayload();

        protected virtual void Awake()
        {
            _rectTransform = (RectTransform)transform;

            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }

            if (_canvasGroup == null)
            {
                UnityEngine.Debug.LogWarning($"{GetType().Name} requires a {nameof(CanvasGroup)} on the card prefab.", this);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _originalParent = transform.parent;
            _originalSiblingIndex = transform.GetSiblingIndex();
            _originalAnchoredPosition = _rectTransform.anchoredPosition;
            _eventCamera = eventData.pressEventCamera;
            _dragRoot = GetDragRoot();
            _dropAccepted = false;

            if (_dragRoot == null)
            {
                return;
            }

            Payload = CreateDragPayload();
            CardDragState.Set(Payload);

            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.alpha = 0.85f;
            }

            transform.SetParent(_dragRoot, false);
            MoveToPointer(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            MoveToPointer(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.alpha = 1f;
            }

            if (_originalParent == null)
            {
                return;
            }

            if (_dropAccepted)
            {
                _dropAccepted = false;
                return;
            }

            transform.SetParent(_originalParent, false);
            transform.SetSiblingIndex(_originalSiblingIndex);
            _rectTransform.anchoredPosition = _originalAnchoredPosition;
        }

        public void AcceptDrop(RectTransform parent, Vector2 anchoredPosition)
        {
            if (parent == null)
            {
                return;
            }

            _dropAccepted = true;
            transform.SetParent(parent, false);
            _rectTransform.anchoredPosition = anchoredPosition;
        }

        private void MoveToPointer(PointerEventData eventData)
        {
            if (_dragRoot == null)
            {
                return;
            }

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _dragRoot,
                    eventData.position,
                    _eventCamera,
                    out Vector2 localPoint))
            {
                _rectTransform.anchoredPosition = localPoint;
            }
        }

        private RectTransform GetDragRoot()
        {
            RectTransform dragRoot = DragLayerView.GetDragRoot();

            if (dragRoot != null)
            {
                return dragRoot;
            }

            return null;
        }
    }
}
