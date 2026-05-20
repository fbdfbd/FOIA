using UnityEngine;

namespace FOIA.Presentation.Views.Cards
{
    public sealed class DragLayerView : ViewBase
    {
        [SerializeField] private RectTransform _dragRoot;

        public static DragLayerView Current { get; private set; }
        public RectTransform DragRoot => _dragRoot != null ? _dragRoot : (RectTransform)transform;

        public static RectTransform GetDragRoot()
        {
            if (Current != null)
            {
                return Current.DragRoot;
            }

            UnityEngine.Debug.LogWarning($"{nameof(DragLayerView)} is missing from the scene.");
            return null;
        }

        private void Awake()
        {
            Current = this;
        }

        private void OnDestroy()
        {
            if (Current == this)
            {
                Current = null;
            }
        }
    }
}
