using UnityEngine;

namespace OneMoreSpoon.View.Substances
{
    public readonly struct SubstanceOutlineColors
    {
        public SubstanceOutlineColors(Color normal, Color selected)
        {
            Normal = normal;
            Selected = selected;
        }

        public Color Normal { get; }
        public Color Selected { get; }

        public static SubstanceOutlineColors Default => new(Color.black, Color.yellow);
    }
}
