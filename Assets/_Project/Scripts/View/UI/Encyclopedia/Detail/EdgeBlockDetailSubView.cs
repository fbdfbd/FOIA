using OneMoreSpoon.App.Encyclopedia.Data;
using TMPro;
using UnityEngine;

namespace OneMoreSpoon.View.UI.Encyclopedia.Detail
{
    public sealed class EdgeBlockDetailSubView : MonoBehaviour
    {
        [SerializeField] private TMP_Text ingredientsText;

        public void Show(EdgeBlockDetailData data)
        {
            ingredientsText.text = data.InputSubstanceNames.Count > 0
                ? string.Join(" + ", data.InputSubstanceNames)
                : string.Empty;

            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);
    }
}
