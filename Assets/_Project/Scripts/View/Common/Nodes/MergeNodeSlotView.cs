using UnityEngine;
using TMPro;

namespace OneMoreSpoon.View.Nodes
{
    public sealed class MergeNodeSlotView : MonoBehaviour
    {
        [SerializeField] private TMP_Text leftSub;
        [SerializeField] private TMP_Text rightSub;

        public void SetSlotText(int index, string text)
        {
            TMP_Text target = index == 0 ? leftSub : rightSub;

            if (target != null) target.text = text;
        }
    }
}
