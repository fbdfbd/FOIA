using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FOIA.Presentation.Views.Debug
{
    public sealed class CardDebugControlView : ViewBase
    {
        [SerializeField] private TextMeshProUGUI _nextCardText;
        [SerializeField] private TextMeshProUGUI _lastResultText;
        [SerializeField] private Button _spawnCardButton;

        public Observable<Unit> OnSpawnCardClicked => _spawnCardButton.OnClickAsObservable();

        public void SetNextCard(string text)
        {
            _nextCardText.text = text;
        }

        public void SetLastResult(string text)
        {
            _lastResultText.text = text;
        }

        public void SetSpawnCardEnabled(bool isEnabled)
        {
            _spawnCardButton.interactable = isEnabled;
        }
    }
}
