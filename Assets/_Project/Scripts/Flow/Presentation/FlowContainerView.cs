using System.Collections.Generic;
using FOIA.Flow.Runtime;
using FOIA.Graph.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FOIA.Flow.Presentation
{
    [DisallowMultipleComponent]
    public sealed class FlowContainerView : MonoBehaviour
    {
        [SerializeField] private FlowRuntimeStore flowStore;
        [SerializeField] private string containerId = "intake";
        [SerializeField] private RectTransform contentRoot;
        [SerializeField] private FlowItemCardView cardPrefab;
        [SerializeField] private Vector2 cardSize = new(180f, 72f);
        [SerializeField] private Vector2 spacing = new(8f, 8f);

        private readonly List<FlowItemCardView> cards = new();

        private void Awake()
        {
            if (flowStore == null)
            {
                flowStore = GraphSceneLookup.FindFirst<FlowRuntimeStore>();
            }

            if (contentRoot == null)
            {
                contentRoot = (RectTransform)transform;
            }

            EnsureLayout();
        }

        private void OnEnable()
        {
            if (flowStore != null)
            {
                flowStore.ItemsChanged += Refresh;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (flowStore != null)
            {
                flowStore.ItemsChanged -= Refresh;
            }
        }

        public void Refresh()
        {
            if (flowStore == null || contentRoot == null)
            {
                return;
            }

            int cardIndex = 0;

            foreach (FlowItem item in flowStore.Items)
            {
                if (item.ContainerId != containerId)
                {
                    continue;
                }

                FlowItemCardView card = GetCard(cardIndex);
                card.Initialize(flowStore);
                card.Bind(item);
                card.gameObject.SetActive(true);
                cardIndex++;
            }

            for (int i = cardIndex; i < cards.Count; i++)
            {
                cards[i].gameObject.SetActive(false);
            }
        }

        private FlowItemCardView GetCard(int index)
        {
            while (cards.Count <= index)
            {
                cards.Add(CreateCard());
            }

            return cards[index];
        }

        private FlowItemCardView CreateCard()
        {
            FlowItemCardView card = cardPrefab != null
                ? Instantiate(cardPrefab, contentRoot, false)
                : FlowItemCardView.CreateDefault(contentRoot);

            RectTransform cardRect = (RectTransform)card.transform;
            cardRect.sizeDelta = cardSize;
            return card;
        }

        private void EnsureLayout()
        {
            if (contentRoot == null)
            {
                return;
            }

            GridLayoutGroup grid = contentRoot.GetComponent<GridLayoutGroup>();

            if (grid == null)
            {
                grid = contentRoot.gameObject.AddComponent<GridLayoutGroup>();
            }

            grid.cellSize = cardSize;
            grid.spacing = spacing;
            grid.childAlignment = TextAnchor.UpperLeft;
            grid.constraint = GridLayoutGroup.Constraint.Flexible;

            if (!contentRoot.TryGetComponent(out ContentSizeFitter fitter))
            {
                fitter = contentRoot.gameObject.AddComponent<ContentSizeFitter>();
            }

            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
}
