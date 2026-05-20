using TMPro;
using UnityEngine;

namespace FOIA.Flow.Presentation
{
    public static class FlowTextStyle
    {
        private const string KoreanFontPath = "Fonts & Materials/PyeojinGothic-Bold SDF";

        private static TMP_FontAsset koreanFont;

        public static void Apply(TMP_Text text)
        {
            if (text == null)
            {
                return;
            }

            TMP_FontAsset font = GetKoreanFont();

            if (font != null)
            {
                text.font = font;
            }
        }

        private static TMP_FontAsset GetKoreanFont()
        {
            if (koreanFont == null)
            {
                koreanFont = Resources.Load<TMP_FontAsset>(KoreanFontPath);
            }

            return koreanFont;
        }
    }
}
