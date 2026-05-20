using FOIA.GameLoop.Models;

namespace FOIA.GameLoop.Events
{
    public interface IHourTickHandler
    {
        void OnHourPassed(GameTimeSnapshot time);
    }
}
