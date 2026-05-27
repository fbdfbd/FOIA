using OneMoreSpoon.Game.Definitions;

namespace OneMoreSpoon.App.Encyclopedia
{
    public static class EncyclopediaTabMatcher
    {
        public static bool BelongsToTab(SubstanceKind kind, EncyclopediaTab tab) => tab switch
        {
            EncyclopediaTab.Dish => SubstanceKindRules.IsPersonLike(kind),
            EncyclopediaTab.EdgeBlock => SubstanceKindRules.IsEdgeBlockLike(kind),
            EncyclopediaTab.TraitShard => SubstanceKindRules.IsStanceLike(kind),
            EncyclopediaTab.SourceMaterial => SubstanceKindRules.IsEtcLike(kind),
            _ => false
        };
    }
}
