using System.Collections.Generic;
using OneMoreSpoon.App.Encyclopedia.Data;
using OneMoreSpoon.Game.Definitions;

namespace OneMoreSpoon.App.Encyclopedia
{
    public sealed class EncyclopediaDetailService
    {
        private readonly IEnumerable<IEncyclopediaDetailProvider> providers;

        public EncyclopediaDetailService(IEnumerable<IEncyclopediaDetailProvider> providers)
        {
            this.providers = providers;
        }

        public EncyclopediaDetailData GetData(SO_SubstanceDefinition definition)
        {
            foreach (var provider in providers)
                if (provider.CanHandle(definition.Kind))
                    return provider.BuildData(definition);

            return null;
        }
    }
}
