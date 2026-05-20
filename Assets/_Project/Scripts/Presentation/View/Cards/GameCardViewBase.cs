using FOIA.Cards.Runtime;
using TMPro;
using UnityEngine;

namespace FOIA.Presentation.Views.Cards
{
    public abstract class GameCardViewBase : DraggableCardViewBase
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _statusText;

        public string RuntimeId { get; private set; }

        public virtual void SetCard(IGameCardRuntime card)
        {
            RuntimeId = card.RuntimeId;
            _titleText.text = card.Title;
            _statusText.text = card.Location.ToString();
        }
    }
}
