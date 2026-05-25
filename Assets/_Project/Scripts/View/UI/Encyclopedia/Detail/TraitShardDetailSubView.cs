using OneMoreSpoon.App.Encyclopedia.Data;
using UnityEngine;

namespace OneMoreSpoon.View.UI.Encyclopedia.Detail
{
    public sealed class TraitShardDetailSubView : MonoBehaviour
    {
        public void Show(TraitShardDetailData data) => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);
    }
}
