using OneMoreSpoon.Game.Systems;
using UnityEngine;
using VContainer.Unity;

namespace OneMoreSpoon.App.Loop
{
    public sealed class GameLoopRunner : ITickable
    {
        private readonly ProcessSystem processSystem;
        private readonly MergeSystem mergeSystem;

        public GameLoopRunner(
            ProcessSystem processSystem,
            MergeSystem mergeSystem)
        {
            this.processSystem = processSystem;
            this.mergeSystem = mergeSystem;
        }

        public void Tick()
        {
            processSystem.Tick(Time.deltaTime);
            mergeSystem.Tick(Time.deltaTime);
        }
    }
}
