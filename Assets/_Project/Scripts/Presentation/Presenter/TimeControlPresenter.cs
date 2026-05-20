using FOIA.GameLoop.State;
using FOIA.GameLoop.Systems;
using FOIA.Presentation.Views;
using R3;

namespace FOIA.Presentation.Presenters
{
    public sealed class TimeControlPresenter : PresenterBase
    {
        private readonly TimeControlView _view;
        private readonly GameCalendarState _calendarState;
        private readonly IGameLoopSystem _gameLoopSystem;

        public TimeControlPresenter(
            TimeControlView view,
            GameCalendarState calendarState,
            IGameLoopSystem gameLoopSystem)
        {
            _view = view;
            _calendarState = calendarState;
            _gameLoopSystem = gameLoopSystem;
        }

        protected override void OnInitialize()
        {
            Refresh();

            Disposables.Add(_view.OnPass1HourClicked.Subscribe(_ => _gameLoopSystem.AdvanceOneHour()));
            Disposables.Add(_view.OnPass3HourClicked.Subscribe(_ => _gameLoopSystem.AdvanceThreeHours()));
            Disposables.Add(_view.OnPass6HourClicked.Subscribe(_ => _gameLoopSystem.AdvanceSixHours()));
            Disposables.Add(_view.OnLeaveWorkClicked.Subscribe(_ => _gameLoopSystem.LeaveWork()));
            Disposables.Add(_view.OnNextDayClicked.Subscribe(_ => _gameLoopSystem.StartNextDay()));

            Disposables.Add(_calendarState.OnDayChanged.Subscribe(_ => Refresh()));
            Disposables.Add(_calendarState.OnHourChanged.Subscribe(_ => Refresh()));
            Disposables.Add(_calendarState.OnPhaseChanged.Subscribe(_ => Refresh()));
            Disposables.Add(_calendarState.OnCanLeaveWorkChanged.Subscribe(_ => Refresh()));
        }

        private void Refresh()
        {
            _view.SetDay(_calendarState.CurrentDay);
            _view.SetHour(_calendarState.CurrentHour);
            _view.SetPhase(_calendarState.CurrentPhase.ToString());
            _view.SetLeaveWorkEnabled(_calendarState.CanLeaveWork);
            _view.SetNextDayEnabled(_calendarState.IsDayEnded);
        }
    }
}