using OneMoreSpoon.Game.Definitions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.View.Substances
{
    [CreateAssetMenu(
        fileName = "SO_SubstanceOutlineConfig",
        menuName = "OneMoreSpoon/View/Substance Outline Config")]
    public sealed class SO_SubstanceOutlineConfig : ScriptableObject
    {
        [SerializeField] private Color fallbackNormalColor = Color.black;
        [SerializeField] private Color fallbackSelectedColor = Color.yellow;
        [SerializeField] private List<Entry> entries = new();

        public SubstanceOutlineColors GetColors(SubstanceKind kind)
        {
            foreach (var entry in entries)
            {
                if (entry == null)
                    continue;

                if (entry.Kind == kind)
                    return new SubstanceOutlineColors(entry.NormalOutlineColor, entry.SelectedOutlineColor);
            }

            return new SubstanceOutlineColors(fallbackNormalColor, fallbackSelectedColor);
        }

        [Serializable]
        private sealed class Entry
        {
            [SerializeField] private SubstanceKind kind;
            [SerializeField] private Color normalOutlineColor = Color.black;
            [SerializeField] private Color selectedOutlineColor = Color.yellow;

            public SubstanceKind Kind => kind;
            public Color NormalOutlineColor => normalOutlineColor;
            public Color SelectedOutlineColor => selectedOutlineColor;
        }
    }
}
