using System;
using System.Collections.Generic;
using OneMoreSpoon.App.Encyclopedia;
using OneMoreSpoon.App.Encyclopedia.Data;
using UnityEngine;

namespace OneMoreSpoon.View.UI.Encyclopedia
{
    public sealed class EncyclopediaTabBadgeView : MonoBehaviour
    {
        [SerializeField] private TabBadgeBinding[] bindings;

        public void Bind(IReadOnlyList<EncyclopediaTabBadgeData> badges)
        {
            foreach (var binding in bindings)
            {
                if (binding.Badge == null)
                    continue;

                binding.Badge.SetActive(FindHasNewEntry(badges, binding.Tab));
            }
        }

        private static bool FindHasNewEntry(IReadOnlyList<EncyclopediaTabBadgeData> badges, EncyclopediaTab tab)
        {
            foreach (var badge in badges)
            {
                if (badge.Tab == tab)
                    return badge.HasNewEntry;
            }

            return false;
        }

        [Serializable]
        private sealed class TabBadgeBinding
        {
            [SerializeField] private EncyclopediaTab tab;
            [SerializeField] private GameObject badge;

            public EncyclopediaTab Tab => tab;
            public GameObject Badge => badge;
        }
    }
}
