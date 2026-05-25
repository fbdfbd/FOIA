using System;
using System.Collections.Generic;
using OneMoreSpoon.App.Encyclopedia;
using OneMoreSpoon.App.Encyclopedia.Data;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.View.UI.Encyclopedia;
using VContainer.Unity;

namespace OneMoreSpoon.Presenter
{
    public sealed class EncyclopediaSyncSystem : IInitializable, ITickable, IDisposable
    {
        private readonly DiscoveryState discoveryState;
        private readonly DiscoveryService discoveryService;
        private readonly EncyclopediaDetailService detailService;
        private readonly SubstanceDefinitionRegistry substanceDefinitions;
        private readonly EncyclopediaView encyclopediaView;

        private EncyclopediaTab currentTab = EncyclopediaTab.Dish;

        public EncyclopediaSyncSystem(
            DiscoveryState discoveryState,
            DiscoveryService discoveryService,
            EncyclopediaDetailService detailService,
            SubstanceDefinitionRegistry substanceDefinitions,
            EncyclopediaView encyclopediaView)
        {
            this.discoveryState = discoveryState;
            this.discoveryService = discoveryService;
            this.detailService = detailService;
            this.substanceDefinitions = substanceDefinitions;
            this.encyclopediaView = encyclopediaView;
        }

        public void Initialize()
        {
            encyclopediaView.OnTabSelected += OnTabSelected;
            encyclopediaView.OnEntrySelected += OnEntrySelected;
            RefreshList();
        }

        public void Tick()
        {
            if (!discoveryState.IsDirty)
                return;

            discoveryState.ClearDirty();
            RefreshList();
        }

        public void Dispose()
        {
            encyclopediaView.OnTabSelected -= OnTabSelected;
            encyclopediaView.OnEntrySelected -= OnEntrySelected;
        }

        private void OnTabSelected(EncyclopediaTab tab)
        {
            currentTab = tab;
            encyclopediaView.HideDetail();
            RefreshList();
        }

        private void OnEntrySelected(string substanceId)
        {
            discoveryService.MarkViewed(substanceId);
            RefreshList();

            if (!substanceDefinitions.TryGet(substanceId, out var def))
                return;

            var data = detailService.GetData(def);
            if (data != null)
                encyclopediaView.ShowDetail(data);
        }

        private void RefreshList()
        {
            var entries = BuildEntries(currentTab);
            encyclopediaView.SetEntries(entries);
        }

        private List<EncyclopediaEntryData> BuildEntries(EncyclopediaTab tab)
        {
            var result = new List<EncyclopediaEntryData>();

            foreach (var def in substanceDefinitions.GetAll())
            {
                if (!BelongsToTab(def.Kind, tab))
                    continue;

                result.Add(new EncyclopediaEntryData(
                    def.SubstanceId,
                    def.DisplayName,
                    discoveryService.IsEncountered(def.SubstanceId),
                    discoveryService.IsNew(def.SubstanceId)));
            }

            return result;
        }

        private static bool BelongsToTab(SubstanceKind kind, EncyclopediaTab tab) => tab switch
        {
            EncyclopediaTab.Dish         => kind == SubstanceKind.Dish || kind == SubstanceKind.FinalDish,
            EncyclopediaTab.EdgeBlock    => kind == SubstanceKind.EdgeBlock,
            EncyclopediaTab.TraitShard   => kind == SubstanceKind.TraitShard,
            EncyclopediaTab.SourceMaterial => kind == SubstanceKind.SourceMaterial,
            _ => false
        };
    }
}
