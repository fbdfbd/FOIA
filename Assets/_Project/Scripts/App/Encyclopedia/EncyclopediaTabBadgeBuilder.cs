using System.Collections.Generic;
using OneMoreSpoon.App.Encyclopedia.Data;
using OneMoreSpoon.Game.Definitions;

namespace OneMoreSpoon.App.Encyclopedia
{
    public static class EncyclopediaTabBadgeBuilder
    {
        private static readonly EncyclopediaTab[] Tabs =
        {
            EncyclopediaTab.Dish,
            EncyclopediaTab.EdgeBlock,
            EncyclopediaTab.TraitShard,
            EncyclopediaTab.SourceMaterial
        };

        public static List<EncyclopediaTabBadgeData> Build(
            SubstanceDefinitionRegistry substanceDefinitions,
            DiscoveryService discoveryService)
        {
            var result = new List<EncyclopediaTabBadgeData>(Tabs.Length);

            foreach (var tab in Tabs)
                result.Add(new EncyclopediaTabBadgeData(tab, HasNewEntry(tab, substanceDefinitions, discoveryService)));

            return result;
        }

        private static bool HasNewEntry(
            EncyclopediaTab tab,
            SubstanceDefinitionRegistry substanceDefinitions,
            DiscoveryService discoveryService)
        {
            foreach (var def in substanceDefinitions.GetAll())
            {
                if (!EncyclopediaTabMatcher.BelongsToTab(def.Kind, tab))
                    continue;

                if (discoveryService.IsNew(def.SubstanceId))
                    return true;
            }

            return false;
        }
    }
}
