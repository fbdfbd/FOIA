using System.Collections.Generic;
using System.Linq;
using FOIA.Cards.Runtime;
using FOIA.Cards.Systems;
using FOIA.Officers.Runtime;
using FOIA.Presentation.Views.Board;
using FOIA.Presentation.Views.Cards;
using FOIA.Presentation.Views.Debug;
using FOIA.Presentation.Views.Inventory;
using FOIA.Requests.Definitions;
using FOIA.Requests.Runtime;
using FOIA.Requests.Sources;
using FOIA.Requests.State;
using FOIA.Requests.Systems;
using R3;

namespace FOIA.Presentation.Presenters
{
    public sealed class CardBoardPresenter : PresenterBase
    {
        private readonly CardInventoryView _cardInventoryView;
        private readonly OfficerInventoryView _officerInventoryView;
        private readonly WorkBoardView _workBoardView;
        private readonly CardDebugControlView _debugControlView;
        private readonly RequestSource _requestSource;
        private readonly RequestSpawnSystem _requestSpawnSystem;
        private readonly RequestAssignmentSystem _requestAssignmentSystem;
        private readonly RequestCaseStore _requestCaseStore;
        private readonly OfficerStore _officerStore;
        private readonly CardPlacementSystem _cardPlacementSystem;
        private readonly Dictionary<string, InventoryGameCardView> _inventoryCards = new();
        private readonly Dictionary<string, BoardGameCardView> _boardCards = new();
        private readonly HashSet<string> _officerCards = new();

        public CardBoardPresenter(
            CardInventoryView cardInventoryView,
            OfficerInventoryView officerInventoryView,
            WorkBoardView workBoardView,
            CardDebugControlView debugControlView,
            RequestSource requestSource,
            RequestSpawnSystem requestSpawnSystem,
            RequestAssignmentSystem requestAssignmentSystem,
            RequestCaseStore requestCaseStore,
            OfficerStore officerStore,
            CardPlacementSystem cardPlacementSystem)
        {
            _cardInventoryView = cardInventoryView;
            _officerInventoryView = officerInventoryView;
            _workBoardView = workBoardView;
            _debugControlView = debugControlView;
            _requestSource = requestSource;
            _requestSpawnSystem = requestSpawnSystem;
            _requestAssignmentSystem = requestAssignmentSystem;
            _requestCaseStore = requestCaseStore;
            _officerStore = officerStore;
            _cardPlacementSystem = cardPlacementSystem;
        }

        protected override void OnInitialize()
        {
            RenderExistingCards();
            RefreshNextCard();

            Disposables.Add(_debugControlView.OnSpawnCardClicked.Subscribe(_ => SpawnRequestCard()));
            Disposables.Add(_workBoardView.OnGameCardDropped.Subscribe(PlaceCardOnBoard));
            Disposables.Add(_officerStore.OnOfficerAdded.Subscribe(CreateOfficerCard));
            Disposables.Add(_requestCaseStore.OnCaseAdded.Subscribe(CreateGameCard));
            Disposables.Add(_requestSource.OnRequestAdded.Subscribe(_ => RefreshNextCard()));
        }

        private void RenderExistingCards()
        {
            foreach (OfficerRuntime officer in _officerStore.Officers)
            {
                CreateOfficerCard(officer);
            }

            foreach (RequestCaseRuntime requestCase in _requestCaseStore.Cases)
            {
                CreateGameCard(requestCase);
            }
        }

        private void SpawnRequestCard()
        {
            if (!_requestSource.HasRequest)
            {
                _debugControlView.SetLastResult("No card definitions.");
                RefreshNextCard();
                return;
            }

            SO_RequestDefinition definition = _requestSource.GetNextRequest();
            RequestCaseRuntime requestCase = _requestSpawnSystem.SpawnRequest(definition);

            _debugControlView.SetLastResult($"Added card: {requestCase.Title}");
            RefreshNextCard();
        }

        private void CreateOfficerCard(OfficerRuntime officer)
        {
            if (!_officerCards.Add(officer.RuntimeId))
            {
                return;
            }

            _officerInventoryView.CreateOfficerCard(officer);
        }

        private void CreateGameCard(RequestCaseRuntime requestCase)
        {
            if (requestCase.Location == CardLocation.Board)
            {
                CreateBoardCard(requestCase);
                return;
            }

            CreateInventoryCard(requestCase);
        }

        private void CreateInventoryCard(RequestCaseRuntime requestCase)
        {
            if (_inventoryCards.ContainsKey(requestCase.RuntimeId))
            {
                return;
            }

            InventoryGameCardView cardView = _cardInventoryView.CreateCard(requestCase);
            _inventoryCards.Add(requestCase.RuntimeId, cardView);
        }

        private void CreateBoardCard(RequestCaseRuntime requestCase)
        {
            if (_boardCards.TryGetValue(requestCase.RuntimeId, out BoardGameCardView existingCard))
            {
                RefreshBoardCard(existingCard, requestCase);
                return;
            }

            BoardGameCardView cardView = _workBoardView.CreateCard(requestCase);
            RefreshBoardCard(cardView, requestCase);
            _boardCards.Add(requestCase.RuntimeId, cardView);

            Disposables.Add(cardView.OnOfficerDropped.Subscribe(officerId => AssignOfficer(requestCase.RuntimeId, officerId)));
        }

        private void PlaceCardOnBoard(WorkBoardCardDrop drop)
        {
            RequestCaseRuntime requestCase = FindRequestCase(drop.RuntimeId);

            if (requestCase == null)
            {
                _debugControlView.SetLastResult("Card move failed.");
                return;
            }

            if (requestCase.Location == CardLocation.Board)
            {
                if (_boardCards.TryGetValue(requestCase.RuntimeId, out BoardGameCardView existingCard))
                {
                    _workBoardView.SetCardPosition(existingCard, drop.AnchoredPosition);
                    _debugControlView.SetLastResult($"Repositioned on board: {requestCase.Title}");
                }

                return;
            }

            _cardPlacementSystem.MoveToBoard(requestCase);

            if (_inventoryCards.TryGetValue(requestCase.RuntimeId, out InventoryGameCardView inventoryCard))
            {
                _cardInventoryView.RemoveCard(inventoryCard);
                _inventoryCards.Remove(requestCase.RuntimeId);
            }

            CreateBoardCard(requestCase);
            if (_boardCards.TryGetValue(requestCase.RuntimeId, out BoardGameCardView boardCard))
            {
                _workBoardView.SetCardPosition(boardCard, drop.AnchoredPosition);
            }

            _debugControlView.SetLastResult($"Moved to board: {requestCase.Title}");
        }

        private void AssignOfficer(string cardId, string officerId)
        {
            RequestCaseRuntime requestCase = FindRequestCase(cardId);
            OfficerRuntime officer = _officerStore.Officers.FirstOrDefault(item => item.RuntimeId == officerId);

            if (requestCase == null || officer == null)
            {
                _debugControlView.SetLastResult("Officer assignment failed.");
                return;
            }

            _requestAssignmentSystem.AssignOfficer(requestCase, officer);

            if (_boardCards.TryGetValue(requestCase.RuntimeId, out BoardGameCardView cardView))
            {
                RefreshBoardCard(cardView, requestCase);
            }

            _debugControlView.SetLastResult($"Assigned {officer.Definition.DisplayName} to {requestCase.Title}.");
        }

        private RequestCaseRuntime FindRequestCase(string runtimeId)
        {
            return _requestCaseStore.Cases.FirstOrDefault(item => item.RuntimeId == runtimeId);
        }

        private static void RefreshBoardCard(BoardGameCardView cardView, RequestCaseRuntime requestCase)
        {
            cardView.SetCard(requestCase);
            cardView.SetAssignedOfficer(requestCase.AssignedOfficerId);
        }

        private void RefreshNextCard()
        {
            _debugControlView.SetSpawnCardEnabled(_requestSource.HasRequest);

            if (!_requestSource.HasRequest)
            {
                _debugControlView.SetNextCard("Next Card: None");
                return;
            }

            _debugControlView.SetNextCard($"Next Card: {_requestSource.PeekNextRequest().Title}");
        }
    }
}
