using FOIA.GameLoop.Models;

namespace FOIA.GameLoop.Events
{
    public interface IPhaseChangedHandler
    {
        void OnPhaseChanged(DayPhase previousPhase, DayPhase currentPhase, GameTimeSnapshot time);
    }
}
