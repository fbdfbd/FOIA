using OneMoreSpoon.App.Encyclopedia.Data;
using UnityEngine;

namespace OneMoreSpoon.View.UI.Encyclopedia.Detail
{
    public sealed class SourceMaterialDetailSubView : MonoBehaviour
    {
        public void Show(SourceMaterialDetailData data) => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);
    }
}
