using OneMoreSpoon.Game.Systems;
using UnityEngine;
using VContainer.Unity;

namespace OneMoreSpoon.App.Loop
{
    public sealed class GameLoopRunner : ITickable
    {
        private readonly ProcessSystem processSystem;

        public GameLoopRunner(ProcessSystem processSystem)
        {
            this.processSystem = processSystem;
        }

        public void Tick()
        {
            processSystem.Tick(Time.deltaTime);
        }
    }
}