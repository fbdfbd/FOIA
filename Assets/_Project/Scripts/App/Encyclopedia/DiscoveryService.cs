using OneMoreSpoon.Game.Definitions;

namespace OneMoreSpoon.App.Encyclopedia
{
    public sealed class DiscoveryService
    {
        private readonly DiscoveryState state;
        private readonly SubstanceDefinitionRegistry substanceDefinitions;

        public DiscoveryService(DiscoveryState state, SubstanceDefinitionRegistry substanceDefinitions)
        {
            this.state = state;
            this.substanceDefinitions = substanceDefinitions;
        }

        public void NotifyEncountered(string substanceId)
        {
            if (!substanceDefinitions.TryGet(substanceId, out var def))
                return;

            if (!IsEncyclopediaKind(def.Kind))
                return;

            state.MarkEncountered(substanceId);
        }

        public void MarkViewed(string substanceId) => state.MarkViewed(substanceId);

        public bool IsEncountered(string substanceId) => state.IsEncountered(substanceId);
        public bool IsNew(string substanceId) => state.IsNew(substanceId);

        private static bool IsEncyclopediaKind(SubstanceKind kind) =>
            kind == SubstanceKind.Dish ||
            kind == SubstanceKind.FinalDish ||
            kind == SubstanceKind.EdgeBlock ||
            kind == SubstanceKind.TraitShard ||
            kind == SubstanceKind.SourceMaterial;
    }
}
