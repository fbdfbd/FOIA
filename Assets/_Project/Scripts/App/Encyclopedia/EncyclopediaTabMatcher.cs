using OneMoreSpoon.Game.Definitions;

namespace OneMoreSpoon.App.Encyclopedia
{
    public static class EncyclopediaTabMatcher
    {
        public static bool BelongsToTab(SubstanceKind kind, EncyclopediaTab tab) => tab switch
        {
            EncyclopediaTab.Dish => kind == SubstanceKind.Dish || kind == SubstanceKind.FinalDish,
            EncyclopediaTab.EdgeBlock => kind == SubstanceKind.EdgeBlock,
            EncyclopediaTab.TraitShard => kind == SubstanceKind.TraitShard,
            EncyclopediaTab.SourceMaterial => kind == SubstanceKind.SourceMaterial,
            _ => false
        };
    }
}
