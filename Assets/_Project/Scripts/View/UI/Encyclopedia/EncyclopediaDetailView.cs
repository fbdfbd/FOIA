using OneMoreSpoon.App.Encyclopedia.Data;
using OneMoreSpoon.View.UI.Encyclopedia.Detail;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OneMoreSpoon.View.UI.Encyclopedia
{
    public sealed class EncyclopediaDetailView : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;

        [SerializeField] private DishDetailSubView dishSubView;
        [SerializeField] private EdgeBlockDetailSubView edgeBlockSubView;
        [SerializeField] private TraitShardDetailSubView traitShardSubView;
        [SerializeField] private SourceMaterialDetailSubView sourceMaterialSubView;

        private void Awake()
        {
            backButton.onClick.AddListener(Hide);
            gameObject.SetActive(false);
        }

        public void Show(EncyclopediaDetailData data)
        {
            titleText.text = data.Title;
            descriptionText.text = data.Description;

            dishSubView.Hide();
            edgeBlockSubView.Hide();
            traitShardSubView.Hide();
            sourceMaterialSubView.Hide();

            switch (data)
            {
                case DishDetailData dish:               dishSubView.Show(dish); break;
                case EdgeBlockDetailData edgeBlock:     edgeBlockSubView.Show(edgeBlock); break;
                case TraitShardDetailData traitShard:   traitShardSubView.Show(traitShard); break;
                case SourceMaterialDetailData source:   sourceMaterialSubView.Show(source); break;
            }

            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);
    }
}
