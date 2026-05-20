using FOIA.Graph.Runtime;
using UnityEngine;

namespace FOIA.Graph.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(EdgeEntity))]
    public sealed class UIEdgeView : MonoBehaviour
    {
        [SerializeField] private float thickness = 12f;

        private EdgeEntity edge;
        private RectTransform rectTransform;
        private RectTransform parentRect;

        public void Initialize(EdgeEntity edgeEntity, float edgeThickness)
        {
            edge = edgeEntity;
            thickness = edgeThickness;
            ResolveRects();
            UpdateLine();
        }

        private void Awake()
        {
            edge = GetComponent<EdgeEntity>();
            ResolveRects();
        }

        private void LateUpdate()
        {
            UpdateLine();
        }

        private void ResolveRects()
        {
            rectTransform = (RectTransform)transform;
            parentRect = rectTransform.parent as RectTransform;
        }

        private void UpdateLine()
        {
            if (edge == null || edge.FromNode == null || edge.ToNode == null || parentRect == null)
            {
                return;
            }

            Vector2 fromPosition = GetLocalPosition(edge.FromNode.ConnectionAnchor);
            Vector2 toPosition = GetLocalPosition(edge.ToNode.ConnectionAnchor);
            Vector2 delta = toPosition - fromPosition;

            rectTransform.anchoredPosition = fromPosition + delta * 0.5f;
            rectTransform.sizeDelta = new Vector2(delta.magnitude, thickness);
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        }

        private Vector2 GetLocalPosition(RectTransform anchor)
        {
            Vector3 worldPosition = anchor.TransformPoint(anchor.rect.center);
            return parentRect.InverseTransformPoint(worldPosition);
        }
    }
}
