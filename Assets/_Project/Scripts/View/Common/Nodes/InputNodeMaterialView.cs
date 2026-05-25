using TMPro;
using UnityEngine;

namespace OneMoreSpoon.View.Nodes
{
    public sealed class InputNodeMaterialView : MonoBehaviour
    {
        [SerializeField] private TMP_Text materialNameText;

        public void SetMaterialName(string materialName)
        {
            if (materialNameText == null)
                return;

            materialNameText.text = materialName;
        }
    }
}