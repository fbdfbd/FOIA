using System.Collections.Generic;
using System.Linq;
using FOIA.GameLoop.Events;
using FOIA.GameLoop.Models;
using FOIA.GameLoop.State;

namespace FOIA.GameLoop.Systems
{
    public sealed class DayEndSystem : IDayEndSystem
    {
        private readonly GameCalendarState _calendarState;
        private readonly IDayEndedHandler[] _dayEndedHandlers;

        public DayEndSystem(
            GameCalendarState calendarState,
            IEnumerable<IDayEndedHandler> dayEndedHandlers)
        {
            _calendarState = calendarState;
            _dayEndedHandlers = dayEndedHandlers.ToArray();
        }

        public bool CanLeaveWork()
        {
            return _calendarState.Snapshot.CanLeaveWork;
        }

        public void LeaveWork()
        {
            if (!CanLeaveWork())
            {
                return;
            }

            _calendarState.SetTime(_calendarState.CurrentDay, GameTimeRules.DayEndHour);

            foreach (IDayEndedHandler handler in _dayEndedHandlers)
            {
                handler.OnDayEnded(DayEndReason.LeaveWork, _calendarState.Snapshot);
            }
        }

        public void StartNextDay()
        {
            _calendarState.StartNextDay();
        }
    }
}
