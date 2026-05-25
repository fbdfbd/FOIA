using UnityEngine;

namespace OneMoreSpoon.View.UI
{
    public sealed class TutorialGoalView : MonoBehaviour
    {
        [SerializeField] private SlidePanelView slidePanel;

        public bool WasOpened;

        private void Awake()
        {
            slidePanel.OnOpened += HandleOpened;
        }

        private void OnDestroy()
        {
            slidePanel.OnOpened -= HandleOpened;
        }

        private void HandleOpened()
        {
            WasOpened = true;
        }
    }
}