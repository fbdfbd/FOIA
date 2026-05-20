namespace FOIA.Cards.Runtime
{
    public interface IGameCardRuntime
    {
        string RuntimeId { get; }
        string Title { get; }
        CardLocation Location { get; }

        void SetLocation(CardLocation location);
    }
}
