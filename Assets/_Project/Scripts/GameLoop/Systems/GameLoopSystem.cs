using FOIA.GameLoop.State;

namespace FOIA.GameLoop.Systems
{
    public sealed class GameLoopSystem : IGameLoopSystem
    {
        private readonly GameCalendarState _calendarState;
        private readonly ITimeAdvanceSystem _timeAdvanceSystem;
        private readonly IDayEndSystem _dayEndSystem;

        public GameLoopSystem(
            GameCalendarState calendarState,
            ITimeAdvanceSystem timeAdvanceSystem,
            IDayEndSystem dayEndSystem)
        {
            _calendarState = calendarState;
            _timeAdvanceSystem = timeAdvanceSystem;
            _dayEndSystem = dayEndSystem;
        }

        public void StartNewGame()
        {
            _calendarState.StartNewGame();
        }

        public void AdvanceOneHour()
        {
            _timeAdvanceSystem.Advance(1);
        }

        public void AdvanceThreeHours()
        {
            _timeAdvanceSystem.Advance(3);
        }

        public void AdvanceSixHours()
        {
            _timeAdvanceSystem.Advance(6);
        }

        public void LeaveWork()
        {
            _dayEndSystem.LeaveWork();
        }

        public void StartNextDay()
        {
            _dayEndSystem.StartNextDay();
        }
    }
}
