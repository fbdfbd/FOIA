using UnityEngine;

namespace FOIA.Presentation
{
    public abstract class ViewBase : MonoBehaviour
    {
        public virtual void InitializeView()
        {
        }

        public virtual void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
    }
}
