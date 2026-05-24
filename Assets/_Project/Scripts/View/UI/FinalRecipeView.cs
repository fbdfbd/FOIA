using UnityEngine;
using UnityEngine.UI;

namespace OneMoreSpoon.View.UI
{
    public sealed class FinalRecipeView : MonoBehaviour
    {
        [SerializeField] private SlidePanelView slidePanel;
        [SerializeField] private Button openButton;

        private void Awake()
        {
            openButton.onClick.AddListener(slidePanel.Open);
        }
    }
}
