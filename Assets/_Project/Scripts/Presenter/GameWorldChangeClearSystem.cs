using OneMoreSpoon.Game.Core;
using VContainer.Unity;

namespace OneMoreSpoon.Presenter
{
    public sealed class GameWorldChangeClearSystem : ITickable
    {
        private readonly GameWorldChanges changes;

        public GameWorldChangeClearSystem(GameWorldChanges changes)
        {
            this.changes = changes;
        }

        public void Tick()
        {
            changes.Clear();
        }
    }
}
