namespace FOIA.GameLoop.Models
{
    public readonly struct TimeAdvanceResult
    {
        public readonly int RequestedHours;
        public readonly int AdvancedHours;
        public readonly GameTimeSnapshot Time;
        public readonly bool IsDayEnded;

        public TimeAdvanceResult(int requestedHours, int advancedHours, GameTimeSnapshot time, bool isDayEnded)
        {
            RequestedHours = requestedHours;
            AdvancedHours = advancedHours;
            Time = time;
            IsDayEnded = isDayEnded;
        }
    }
}
