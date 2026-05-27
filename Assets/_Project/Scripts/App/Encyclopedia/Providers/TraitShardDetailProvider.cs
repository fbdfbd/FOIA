using OneMoreSpoon.App.Encyclopedia.Data;
using OneMoreSpoon.Game.Definitions;

namespace OneMoreSpoon.App.Encyclopedia.Providers
{
    public sealed class TraitShardDetailProvider : IEncyclopediaDetailProvider
    {
        private readonly SubstanceInspectDefinitionRegistry inspectDefinitions;

        public TraitShardDetailProvider(SubstanceInspectDefinitionRegistry inspectDefinitions)
        {
            this.inspectDefinitions = inspectDefinitions;
        }

        public bool CanHandle(SubstanceKind kind) => SubstanceKindRules.IsStanceLike(kind);

        public EncyclopediaDetailData BuildData(SO_SubstanceDefinition definition)
        {
            inspectDefinitions.TryGet(definition.SubstanceId, out var inspectDef);
            var description = inspectDef?.Description ?? string.Empty;
            return new TraitShardDetailData(definition.DisplayName, description);
        }
    }
}
