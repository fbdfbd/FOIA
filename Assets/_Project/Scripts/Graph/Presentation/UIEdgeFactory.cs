using FOIA.Graph.Runtime;
using FOIA.Graph.Input;
using FOIA.Flow.Presentation;
using FOIA.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace FOIA.Graph.Presentation
{
    public sealed class UIEdgeFactory : MonoBehaviour
    {
        [SerializeField] private RectTransform edgeRoot;
        [SerializeField] private EdgeEntity edgePrefab;
        [SerializeField] private Color edgeColor = Color.white;
        [SerializeField] private float edgeThickness = 12f;

        public EdgeEntity CreateEdge(EdgeRuntimeData data, NodeEntity fromNode, NodeEntity toNode, GraphRuntimeStore graphStore)
        {
            EdgeEntity edge = edgePrefab != null
                ? Instantiate(edgePrefab, GetEdgeRoot(), false)
                : CreateDefaultEdge(GetEdgeRoot());

            EnsureComponent<UIClickable>(edge.gameObject);
            EnsureComponent<UIRightClickable>(edge.gameObject);
            EnsureComponent<UIEdgeView>(edge.gameObject);
            EnsureComponent<UIEdgeDirectionView>(edge.gameObject);
            EnsureComponent<EdgeSelectable>(edge.gameObject);
            EnsureComponent<EdgeContextInput>(edge.gameObject);
            EnsureComponent<EdgeFlowBlockView>(edge.gameObject);

            edge.Initialize(data, fromNode, toNode);

            UIEdgeView view = edge.GetComponent<UIEdgeView>();
            if (view != null)
            {
                view.Initialize(edge, edgeThickness);
            }

            UIEdgeDirectionView directionView = edge.GetComponent<UIEdgeDirectionView>();
            if (directionView != null)
            {
                directionView.Initialize(edge, edgeColor);
            }

            EdgeSelectable selectable = edge.GetComponent<EdgeSelectable>();
            if (selectable != null)
            {
                selectable.Initialize(graphStore, edge);
            }

            EdgeContextInput contextInput = edge.GetComponent<EdgeContextInput>();
            if (contextInput != null)
            {
                contextInput.Initialize(graphStore, edge);
            }

            return edge;
        }

        private RectTransform GetEdgeRoot()
        {
            if (edgeRoot != null)
            {
                return edgeRoot;
            }

            return (RectTransform)transform;
        }

        private EdgeEntity CreateDefaultEdge(RectTransform root)
        {
            GameObject edgeObject = new GameObject("Edge", typeof(RectTransform), typeof(Image));
            RectTransform rectTransform = (RectTransform)edgeObject.transform;
            rectTransform.SetParent(root, false);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            Image image = edgeObject.GetComponent<Image>();
            image.color = edgeColor;
            image.raycastTarget = true;

            edgeObject.AddComponent<UIClickable>();
            edgeObject.AddComponent<UIRightClickable>();
            edgeObject.AddComponent<EdgeEntity>();
            edgeObject.AddComponent<UIEdgeView>();
            edgeObject.AddComponent<UIEdgeDirectionView>();
            edgeObject.AddComponent<EdgeSelectable>();
            edgeObject.AddComponent<EdgeContextInput>();
            edgeObject.AddComponent<EdgeFlowBlockView>();
            return edgeObject.GetComponent<EdgeEntity>();
        }

        private static T EnsureComponent<T>(GameObject target) where T : Component
        {
            if (!target.TryGetComponent(out T component))
            {
                component = target.AddComponent<T>();
            }

            return component;
        }
    }
}
