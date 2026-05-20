using System;

namespace FOIA.GameLoop.Models
{
    public static class GameTimeRules
    {
        public const int DayStartHour = 0;
        public const int WorkStartHour = 9;
        public const int WorkEndHour = 18;
        public const int DayEndHour = 24;

        public static DayPhase GetPhase(int hour)
        {
            if (hour < WorkStartHour)
            {
                return DayPhase.DayStart;
            }

            if (hour < WorkEndHour)
            {
                return DayPhase.WorkHours;
            }

            if (hour < DayEndHour)
            {
                return DayPhase.AfterHours;
            }

            return DayPhase.DayEnd;
        }

        public static int ClampAdvanceHours(int currentHour, int requestedHours)
        {
            if (requestedHours <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requestedHours), "Advance hours must be greater than zero.");
            }

            return Math.Min(requestedHours, DayEndHour - currentHour);
        }
    }
}
