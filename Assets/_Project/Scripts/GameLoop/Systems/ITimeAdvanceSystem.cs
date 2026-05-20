using FOIA.GameLoop.Models;

namespace FOIA.GameLoop.Systems
{
    public interface ITimeAdvanceSystem
    {
        bool CanAdvance(int hours);
        TimeAdvanceResult Advance(int hours);
    }
}
