namespace FOIA.GameLoop.Systems
{
    public interface IGameLoopSystem
    {
        void StartNewGame();
        void AdvanceOneHour();
        void AdvanceThreeHours();
        void AdvanceSixHours();
        void LeaveWork();
        void StartNextDay();
    }
}
