using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace OneMoreSpoon.View.UI
{
    public sealed class SlidePanelView : MonoBehaviour
    {
        [SerializeField] private float closedX = -600f;
        [SerializeField] private float openX = 0f;
        [SerializeField] private float slideDuration = 0.3f;
        [SerializeField] private Ease slideEase = Ease.OutCubic;
        [SerializeField] private Button overlayButton;

        public event Action OnOpened;
        public event Action OnClosed;

        public bool IsOpen { get; private set; }

        private RectTransform panelRect;
        private Tween slideTween;

        private void Awake()
        {
            panelRect = GetComponent<RectTransform>();

            SetPositionImmediate(closedX);
            overlayButton.gameObject.SetActive(false);
            overlayButton.onClick.AddListener(Close);
        }

        public void Open()
        {
            if (IsOpen) return;
            IsOpen = true;
            overlayButton.gameObject.SetActive(true);
            SlideTo(openX);
            OnOpened?.Invoke();
        }

        public void Close()
        {
            if (!IsOpen) return;
            IsOpen = false;
            overlayButton.gameObject.SetActive(false);
            SlideTo(closedX);
            OnClosed?.Invoke();
        }

        private void SlideTo(float targetX)
        {
            slideTween?.Kill();
            slideTween = panelRect.DOAnchorPosX(targetX, slideDuration).SetEase(slideEase);
        }

        private void SetPositionImmediate(float x)
        {
            var pos = panelRect.anchoredPosition;
            pos.x = x;
            panelRect.anchoredPosition = pos;
        }
    }
}
