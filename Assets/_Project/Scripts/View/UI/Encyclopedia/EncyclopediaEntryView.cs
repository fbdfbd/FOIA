using System;
using OneMoreSpoon.App.Encyclopedia.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OneMoreSpoon.View.UI.Encyclopedia
{
    public sealed class EncyclopediaEntryView : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private GameObject newBadge;
        [SerializeField] private Button button;

        public event Action<string> OnClicked;

        private string substanceId;

        private void Awake()
        {
            button.onClick.AddListener(() => OnClicked?.Invoke(substanceId));
        }

        public void Bind(EncyclopediaEntryData data)
        {
            substanceId = data.SubstanceId;
            nameText.text = data.IsEncountered ? data.DisplayName : "???";
            newBadge.SetActive(data.IsNew);
            button.interactable = data.IsEncountered;
        }
    }
}
