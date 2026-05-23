using OneMoreSpoon.App.Inspect;
using TMPro;
using UnityEngine;

namespace OneMoreSpoon.View.UI
{
    public sealed class InspectPanelView : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text tagsText;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void Show(InspectPanelData data)
        {
            titleText.text = data.Title;
            descriptionText.text = data.Description;
            tagsText.text = data.Tags.Count > 0 ? string.Join(", ", data.Tags) : string.Empty;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
