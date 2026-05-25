using System;
using System.Collections.Generic;
using OneMoreSpoon.App.Encyclopedia;
using OneMoreSpoon.App.Encyclopedia.Data;
using UnityEngine;
using UnityEngine.UI;

namespace OneMoreSpoon.View.UI.Encyclopedia
{
    public sealed class EncyclopediaView : MonoBehaviour
    {
        [Header("Tabs")]
        [SerializeField] private Button tabDish;
        [SerializeField] private Button tabEdgeBlock;
        [SerializeField] private Button tabTraitShard;
        [SerializeField] private Button tabSourceMaterial;

        [Header("List")]
        [SerializeField] private Transform entryListContent;
        [SerializeField] private EncyclopediaEntryView entryViewPrefab;

        [Header("Detail")]
        [SerializeField] private EncyclopediaDetailView detailView;

        [Header("Slide")]
        [SerializeField] private SlidePanelView slidePanel;

        [Header("Index Badges")]
        [SerializeField] private EncyclopediaTabBadgeView tabBadgeView;

        public event Action<EncyclopediaTab> OnTabSelected;
        public event Action<string> OnEntrySelected;

        private readonly List<EncyclopediaEntryView> pool = new();

        private void Awake()
        {
            slidePanel.OnClosed += HideDetail;

            tabDish.onClick.AddListener(()           => { slidePanel.Open(); OnTabSelected?.Invoke(EncyclopediaTab.Dish); });
            tabEdgeBlock.onClick.AddListener(()      => { slidePanel.Open(); OnTabSelected?.Invoke(EncyclopediaTab.EdgeBlock); });
            tabTraitShard.onClick.AddListener(()     => { slidePanel.Open(); OnTabSelected?.Invoke(EncyclopediaTab.TraitShard); });
            tabSourceMaterial.onClick.AddListener(() => { slidePanel.Open(); OnTabSelected?.Invoke(EncyclopediaTab.SourceMaterial); });
        }

        private void OnDestroy()
        {
            slidePanel.OnClosed -= HideDetail;
        }

        public void SetEntries(IReadOnlyList<EncyclopediaEntryData> entries)
        {
            foreach (var view in pool)
                view.gameObject.SetActive(false);

            for (var i = 0; i < entries.Count; i++)
            {
                if (i >= pool.Count)
                {
                    var newView = Instantiate(entryViewPrefab, entryListContent);
                    newView.OnClicked += id => OnEntrySelected?.Invoke(id);
                    pool.Add(newView);
                }

                pool[i].Bind(entries[i]);
                pool[i].gameObject.SetActive(true);
            }
        }

        public void SetTabBadges(IReadOnlyList<EncyclopediaTabBadgeData> badges) => tabBadgeView?.Bind(badges);

        public void ShowDetail(EncyclopediaDetailData data) => detailView.Show(data);
        public void HideDetail() => detailView.Hide();
    }
}
