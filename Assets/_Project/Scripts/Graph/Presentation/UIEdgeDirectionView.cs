using FOIA.Graph.Runtime;
using UnityEngine;

namespace FOIA.Graph.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(EdgeEntity))]
    public sealed class UIEdgeDirectionView : MonoBehaviour
    {
        [SerializeField] private Color arrowColor = Color.white;
        [SerializeField] private Vector2 arrowSize = new(22f, 18f);
        [SerializeField] private float endpointOffset = 10f;

        private EdgeEntity edge;
        private RectTransform rectTransform;
        private UIEdgeArrowhead forwardArrow;
        private UIEdgeArrowhead backwardArrow;

        public void Initialize(EdgeEntity edgeEntity, Color color)
        {
            edge = edgeEntity;
            arrowColor = color;
            ResolveRects();
            EnsureArrows();
            UpdateDirection();
        }

        private void Awake()
        {
            edge = GetComponent<EdgeEntity>();
            ResolveRects();
            EnsureArrows();
        }

        private void LateUpdate()
        {
            UpdateDirection();
        }

        private void ResolveRects()
        {
            rectTransform = (RectTransform)transform;
        }

        private void EnsureArrows()
        {
            forwardArrow = EnsureArrow(forwardArrow, "ForwardArrow");
            backwardArrow = EnsureArrow(backwardArrow, "BackwardArrow");
        }

        private UIEdgeArrowhead EnsureArrow(UIEdgeArrowhead arrow, string arrowName)
        {
            if (arrow != null)
            {
                return arrow;
            }

            Transform existing = transform.Find(arrowName);
            if (existing != null && existing.TryGetComponent(out UIEdgeArrowhead existingArrow))
            {
                ConfigureArrow(existingArrow);
                return existingArrow;
            }

            GameObject arrowObject = new(arrowName, typeof(RectTransform), typeof(CanvasRenderer), typeof(UIEdgeArrowhead));
            RectTransform arrowRect = (RectTransform)arrowObject.transform;
            arrowRect.SetParent(rectTransform, false);
            arrowRect.anchorMin = new Vector2(0.5f, 0.5f);
            arrowRect.anchorMax = new Vector2(0.5f, 0.5f);
            arrowRect.pivot = new Vector2(0.5f, 0.5f);

            UIEdgeArrowhead createdArrow = arrowObject.GetComponent<UIEdgeArrowhead>();
            ConfigureArrow(createdArrow);
            return createdArrow;
        }

        private void ConfigureArrow(UIEdgeArrowhead arrow)
        {
            RectTransform arrowRect = (RectTransform)arrow.transform;
            arrowRect.sizeDelta = arrowSize;
            arrow.color = arrowColor;
            arrow.raycastTarget = false;
        }

        private void UpdateDirection()
        {
            if (edge == null || edge.Data == null)
            {
                SetArrowVisibility(false, false);
                return;
            }

            float edgeLength = rectTransform.rect.width;

            if (edgeLength <= Mathf.Epsilon)
            {
                SetArrowVisibility(false, false);
                return;
            }

            EdgeDirection direction = edge.Data.Direction;
            bool showForward = direction == EdgeDirection.Forward || direction == EdgeDirection.Bidirectional;
            bool showBackward = direction == EdgeDirection.Backward || direction == EdgeDirection.Bidirectional;
            SetArrowVisibility(showForward, showBackward);

            if (showForward)
            {
                PositionArrow(forwardArrow, edgeLength * 0.5f - endpointOffset, 1f);
            }

            if (showBackward)
            {
                PositionArrow(backwardArrow, -edgeLength * 0.5f + endpointOffset, -1f);
            }
        }

        private void SetArrowVisibility(bool showForward, bool showBackward)
        {
            if (forwardArrow != null)
            {
                forwardArrow.gameObject.SetActive(showForward);
            }

            if (backwardArrow != null)
            {
                backwardArrow.gameObject.SetActive(showBackward);
            }
        }

        private void PositionArrow(UIEdgeArrowhead arrow, float tipX, float direction)
        {
            if (arrow == null)
            {
                return;
            }

            RectTransform arrowRect = (RectTransform)arrow.transform;
            arrowRect.anchoredPosition = new Vector2(tipX - direction * arrowRect.sizeDelta.x * 0.5f, 0f);
            arrowRect.localRotation = Quaternion.Euler(0f, 0f, direction > 0f ? 0f : 180f);
            arrow.color = arrowColor;
        }
    }
}
