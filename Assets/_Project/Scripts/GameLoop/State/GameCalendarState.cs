using FOIA.GameLoop.Models;
using R3;

namespace FOIA.GameLoop.State
{
    public sealed class GameCalendarState
    {
        private readonly Subject<int> _onDayChanged = new();
        private readonly Subject<int> _onHourChanged = new();
        private readonly Subject<DayPhase> _onPhaseChanged = new();
        private readonly Subject<bool> _onCanLeaveWorkChanged = new();
        private int _day = 1;
        private int _hour = GameTimeRules.WorkStartHour;
        private DayPhase _phase = DayPhase.WorkHours;
        private bool _canLeaveWork;

        public Observable<int> OnDayChanged => _onDayChanged;
        public Observable<int> OnHourChanged => _onHourChanged;
        public Observable<DayPhase> OnPhaseChanged => _onPhaseChanged;
        public Observable<bool> OnCanLeaveWorkChanged => _onCanLeaveWorkChanged;

        public int CurrentDay => _day;
        public int CurrentHour => _hour;
        public DayPhase CurrentPhase => _phase;
        public bool CanLeaveWork => _canLeaveWork;
        public bool IsDayEnded => CurrentPhase == DayPhase.DayEnd;

        public GameTimeSnapshot Snapshot => new(CurrentDay, CurrentHour, CurrentPhase);

        public void StartNewGame()
        {
            SetTime(1, GameTimeRules.WorkStartHour);
        }

        public void StartNextDay()
        {
            SetTime(CurrentDay + 1, GameTimeRules.WorkStartHour);
        }

        public void SetTime(int day, int hour)
        {
            DayPhase nextPhase = GameTimeRules.GetPhase(hour);
            bool nextCanLeaveWork = hour >= GameTimeRules.WorkEndHour && hour < GameTimeRules.DayEndHour;

            SetDay(day);
            SetHour(hour);
            SetPhase(nextPhase);
            SetCanLeaveWork(nextCanLeaveWork);
        }

        private void SetDay(int day)
        {
            if (_day == day)
            {
                return;
            }

            _day = day;
            _onDayChanged.OnNext(_day);
        }

        private void SetHour(int hour)
        {
            if (_hour == hour)
            {
                return;
            }

            _hour = hour;
            _onHourChanged.OnNext(_hour);
        }

        private void SetPhase(DayPhase phase)
        {
            if (_phase == phase)
            {
                return;
            }

            _phase = phase;
            _onPhaseChanged.OnNext(_phase);
        }

        private void SetCanLeaveWork(bool canLeaveWork)
        {
            if (_canLeaveWork == canLeaveWork)
            {
                return;
            }

            _canLeaveWork = canLeaveWork;
            _onCanLeaveWorkChanged.OnNext(_canLeaveWork);
        }
    }
}
