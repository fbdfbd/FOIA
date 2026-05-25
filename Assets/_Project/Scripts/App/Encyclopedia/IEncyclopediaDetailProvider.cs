using OneMoreSpoon.App.Encyclopedia.Data;
using OneMoreSpoon.Game.Definitions;

namespace OneMoreSpoon.App.Encyclopedia
{
    public interface IEncyclopediaDetailProvider
    {
        bool CanHandle(SubstanceKind kind);
        EncyclopediaDetailData BuildData(SO_SubstanceDefinition definition);
    }
}
