using System.Collections.Generic;
using System.Linq;
using FOIA.GameLoop.Events;
using FOIA.GameLoop.Models;
using FOIA.GameLoop.State;
using R3;

namespace FOIA.GameLoop.Systems
{
    public sealed class TimeAdvanceSystem : ITimeAdvanceSystem
    {
        private readonly GameCalendarState _calendarState;
        private readonly IHourTickHandler[] _hourTickHandlers;
        private readonly IPhaseChangedHandler[] _phaseChangedHandlers;
        private readonly IDayEndedHandler[] _dayEndedHandlers;
        private readonly Subject<GameTimeSnapshot> _onHourPassed = new();
        private readonly Subject<TimeAdvanceResult> _onTimeAdvanced = new();

        public Observable<GameTimeSnapshot> OnHourPassed => _onHourPassed;
        public Observable<TimeAdvanceResult> OnTimeAdvanced => _onTimeAdvanced;

        public TimeAdvanceSystem(
            GameCalendarState calendarState,
            IEnumerable<IHourTickHandler> hourTickHandlers,
            IEnumerable<IPhaseChangedHandler> phaseChangedHandlers,
            IEnumerable<IDayEndedHandler> dayEndedHandlers)
        {
            _calendarState = calendarState;
            _hourTickHandlers = hourTickHandlers.ToArray();
            _phaseChangedHandlers = phaseChangedHandlers.ToArray();
            _dayEndedHandlers = dayEndedHandlers.ToArray();
        }

        public bool CanAdvance(int hours)
        {
            return hours > 0 && !_calendarState.IsDayEnded && _calendarState.CurrentHour < GameTimeRules.DayEndHour;
        }

        public TimeAdvanceResult Advance(int hours)
        {
            if (!CanAdvance(hours))
            {
                return new TimeAdvanceResult(hours, 0, _calendarState.Snapshot, _calendarState.IsDayEnded);
            }

            int advancedHours = GameTimeRules.ClampAdvanceHours(_calendarState.CurrentHour, hours);

            for (int i = 0; i < advancedHours; i++)
            {
                AdvanceOneHour();
            }

            TimeAdvanceResult result = new(hours, advancedHours, _calendarState.Snapshot, _calendarState.IsDayEnded);
            _onTimeAdvanced.OnNext(result);
            return result;
        }

        private void AdvanceOneHour()
        {
            DayPhase previousPhase = _calendarState.CurrentPhase;
            int nextHour = _calendarState.CurrentHour + 1;

            _calendarState.SetTime(_calendarState.CurrentDay, nextHour);

            GameTimeSnapshot time = _calendarState.Snapshot;
            NotifyHourPassed(time);

            if (previousPhase != time.Phase)
            {
                NotifyPhaseChanged(previousPhase, time.Phase, time);
            }

            if (time.Phase == DayPhase.DayEnd)
            {
                NotifyDayEnded(DayEndReason.Midnight, time);
            }
        }

        private void NotifyHourPassed(GameTimeSnapshot time)
        {
            foreach (IHourTickHandler handler in _hourTickHandlers)
            {
                handler.OnHourPassed(time);
            }

            _onHourPassed.OnNext(time);
        }

        private void NotifyPhaseChanged(DayPhase previousPhase, DayPhase currentPhase, GameTimeSnapshot time)
        {
            foreach (IPhaseChangedHandler handler in _phaseChangedHandlers)
            {
                handler.OnPhaseChanged(previousPhase, currentPhase, time);
            }
        }

        private void NotifyDayEnded(DayEndReason reason, GameTimeSnapshot time)
        {
            foreach (IDayEndedHandler handler in _dayEndedHandlers)
            {
                handler.OnDayEnded(reason, time);
            }
        }
    }
}
