using OneMoreSpoon.App.Encyclopedia.Data;
using TMPro;
using UnityEngine;

namespace OneMoreSpoon.View.UI.Encyclopedia.Detail
{
    public sealed class DishDetailSubView : MonoBehaviour
    {
        [SerializeField] private TMP_Text inputText;
        [SerializeField] private TMP_Text stepsText;
        [SerializeField] private TMP_Text byproductsText;

        public void Show(DishDetailData data)
        {
            inputText.text = data.InputSubstanceName;
            stepsText.text = data.ProcessSteps.Count > 0
                ? string.Join(" → ", data.ProcessSteps)
                : string.Empty;
            byproductsText.text = data.ByproductNames.Count > 0
                ? string.Join(", ", data.ByproductNames)
                : string.Empty;

            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);
    }
}
