using OneMoreSpoon.App.Encyclopedia.Data;
using OneMoreSpoon.Game.Definitions;

namespace OneMoreSpoon.App.Encyclopedia.Providers
{
    public sealed class SourceMaterialDetailProvider : IEncyclopediaDetailProvider
    {
        private readonly SubstanceInspectDefinitionRegistry inspectDefinitions;

        public SourceMaterialDetailProvider(SubstanceInspectDefinitionRegistry inspectDefinitions)
        {
            this.inspectDefinitions = inspectDefinitions;
        }

        public bool CanHandle(SubstanceKind kind) => kind == SubstanceKind.SourceMaterial;

        public EncyclopediaDetailData BuildData(SO_SubstanceDefinition definition)
        {
            inspectDefinitions.TryGet(definition.SubstanceId, out var inspectDef);
            var description = inspectDef?.Description ?? string.Empty;
            return new SourceMaterialDetailData(definition.DisplayName, description);
        }
    }
}
