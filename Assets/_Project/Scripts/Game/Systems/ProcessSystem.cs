using OneMoreSpoon.Game.Core;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class ProcessSystem
    {
        private readonly GameWorld world;
        private bool created;

        public ProcessSystem(GameWorld world)
        {
            this.world = world;
        }

        public void Tick(float deltaTime)
        {
           
        }
    }
}