using UnityEngine;
using TMPro;

namespace OneMoreSpoon.View.Nodes
{
    public sealed class MergeNodeSlotView : MonoBehaviour
    {
        [SerializeField] private TMP_Text leftSub;
        [SerializeField] private TMP_Text rightSub;

        private string leftText = string.Empty;
        private string rightText = string.Empty;

        public void SetSlotText(int index, string text)
        {
            if (index == 0)
            {
                SetTextIfChanged(leftSub, ref leftText, text);
                return;
            }

            SetTextIfChanged(rightSub, ref rightText, text);
        }

        private static void SetTextIfChanged(TMP_Text target, ref string currentText, string nextText)
        {
            if (target == null || currentText == nextText)
                return;

            currentText = nextText;
            target.text = nextText;
        }
    }
}
