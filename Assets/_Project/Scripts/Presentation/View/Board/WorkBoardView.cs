using FOIA.Cards.Runtime;
using FOIA.Presentation.Views.Cards;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FOIA.Presentation.Views.Board
{
    public readonly struct WorkBoardCardDrop
    {
        public readonly string RuntimeId;
        public readonly Vector2 AnchoredPosition;

        public WorkBoardCardDrop(string runtimeId, Vector2 anchoredPosition)
        {
            RuntimeId = runtimeId;
            AnchoredPosition = anchoredPosition;
        }
    }

    public sealed class WorkBoardView : ViewBase, IDropHandler
    {
        [SerializeField] private RectTransform _cardRoot;
        [SerializeField] private BoardGameCardView _gameCardPrefab;
        [SerializeField] private WorkBoardDropTargetView _dropTarget;

        private readonly Subject<WorkBoardCardDrop> _onGameCardDropped = new();

        public Observable<WorkBoardCardDrop> OnGameCardDropped => _onGameCardDropped;

        private void Awake()
        {
            InitializeDropTarget();
        }

        public BoardGameCardView CreateCard(IGameCardRuntime card)
        {
            BoardGameCardView cardView = Instantiate(_gameCardPrefab, _cardRoot);
            cardView.SetCard(card);
            return cardView;
        }

        public void SetCardPosition(BoardGameCardView cardView, Vector2 anchoredPosition)
        {
            if (cardView == null)
            {
                return;
            }

            RectTransform cardTransform = (RectTransform)cardView.transform;
            cardTransform.anchoredPosition = ClampToCardRoot(cardTransform, anchoredPosition);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (!TryGetPayload(eventData, out CardDragPayload payload))
            {
                return;
            }

            if (payload.Kind != DragKind.GameCard)
            {
                return;
            }

            WorkBoardCardDrop drop = CreateDrop(payload, eventData);
            AcceptBoardCardDrop(eventData, drop.AnchoredPosition);
            _onGameCardDropped.OnNext(drop);
        }

        private void InitializeDropTarget()
        {
            if (_cardRoot == null)
            {
                UnityEngine.Debug.LogWarning($"{nameof(WorkBoardView)} requires a card root.", this);
                return;
            }

            if (_dropTarget == null)
            {
                _dropTarget = _cardRoot.GetComponent<WorkBoardDropTargetView>();
            }

            if (_dropTarget == null)
            {
                UnityEngine.Debug.LogWarning($"{nameof(WorkBoardView)} requires a {nameof(WorkBoardDropTargetView)} on the card root.", this);
                return;
            }

            _dropTarget.Initialize((cardId, eventData) =>
            {
                WorkBoardCardDrop drop = CreateDrop(cardId, eventData);
                AcceptBoardCardDrop(eventData, drop.AnchoredPosition);
                _onGameCardDropped.OnNext(drop);
            });
        }

        private WorkBoardCardDrop CreateDrop(CardDragPayload payload, PointerEventData eventData)
        {
            return CreateDrop(payload.RuntimeId, eventData);
        }

        private WorkBoardCardDrop CreateDrop(string cardId, PointerEventData eventData)
        {
            Vector2 anchoredPosition = Vector2.zero;

            if (_cardRoot != null)
            {
                Camera eventCamera = eventData.pressEventCamera ?? eventData.enterEventCamera;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _cardRoot,
                    eventData.position,
                    eventCamera,
                    out anchoredPosition);
            }

            return new WorkBoardCardDrop(cardId, anchoredPosition);
        }

        private void AcceptBoardCardDrop(PointerEventData eventData, Vector2 anchoredPosition)
        {
            if (eventData.pointerDrag == null)
            {
                return;
            }

            BoardGameCardView boardCard = eventData.pointerDrag.GetComponent<BoardGameCardView>();

            if (boardCard == null)
            {
                return;
            }

            RectTransform cardTransform = (RectTransform)boardCard.transform;
            boardCard.AcceptDrop(_cardRoot, ClampToCardRoot(cardTransform, anchoredPosition));
        }

        private Vector2 ClampToCardRoot(RectTransform cardTransform, Vector2 anchoredPosition)
        {
            if (_cardRoot == null || cardTransform == null)
            {
                return anchoredPosition;
            }

            Rect rootRect = _cardRoot.rect;
            Rect cardRect = cardTransform.rect;

            float minX = rootRect.xMin + cardRect.width * cardTransform.pivot.x;
            float maxX = rootRect.xMax - cardRect.width * (1f - cardTransform.pivot.x);
            float minY = rootRect.yMin + cardRect.height * cardTransform.pivot.y;
            float maxY = rootRect.yMax - cardRect.height * (1f - cardTransform.pivot.y);

            if (minX > maxX)
            {
                minX = maxX = rootRect.center.x;
            }

            if (minY > maxY)
            {
                minY = maxY = rootRect.center.y;
            }

            return new Vector2(
                Mathf.Clamp(anchoredPosition.x, minX, maxX),
                Mathf.Clamp(anchoredPosition.y, minY, maxY));
        }

        private static bool TryGetPayload(PointerEventData eventData, out CardDragPayload payload)
        {
            payload = default;

            if (eventData.pointerDrag == null)
            {
                return false;
            }

            ICardDragPayloadProvider provider = eventData.pointerDrag.GetComponent<ICardDragPayloadProvider>();

            if (provider == null)
            {
                return false;
            }

            payload = provider.Payload;
            return true;
        }
    }
}
