using FOIA.GameLoop.Models;

namespace FOIA.GameLoop.Events
{
    public interface IDayEndedHandler
    {
        void OnDayEnded(DayEndReason reason, GameTimeSnapshot time);
    }
}
