using FOIA.GameLoop.Models;

namespace FOIA.GameLoop.Systems
{
    public interface IDayEndSystem
    {
        bool CanLeaveWork();
        void LeaveWork();
        void StartNextDay();
    }
}
