namespace FOIA.GameLoop.Models
{
    public readonly struct GameTimeSnapshot
    {
        public readonly int Day;
        public readonly int Hour;
        public readonly DayPhase Phase;

        public GameTimeSnapshot(int day, int hour, DayPhase phase)
        {
            Day = day;
            Hour = hour;
            Phase = phase;
        }

        public bool IsWorkHours => Phase == DayPhase.WorkHours;
        public bool IsAfterHours => Phase == DayPhase.AfterHours;
        public bool CanLeaveWork => Hour >= GameTimeRules.WorkEndHour && Hour < GameTimeRules.DayEndHour;
    }
}
