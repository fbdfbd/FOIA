using System.Linq;
using FOIA.Flow.Input;
using FOIA.Flow.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FOIA.Flow.Presentation
{
    [DisallowMultipleComponent]
    public sealed class FlowItemCardView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image background;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text tagText;

        private FlowRuntimeStore flowStore;
        private FlowItem item;

        public static FlowItemCardView CreateDefault(RectTransform parent)
        {
            GameObject cardObject = new("FlowItemCard", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(FlowItemCardView));
            RectTransform cardRect = (RectTransform)cardObject.transform;
            cardRect.SetParent(parent, false);

            GameObject titleObject = new("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            RectTransform titleRect = (RectTransform)titleObject.transform;
            titleRect.SetParent(cardRect, false);
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.offsetMin = new Vector2(10f, -34f);
            titleRect.offsetMax = new Vector2(-10f, -8f);

            GameObject tagObject = new("Tags", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            RectTransform tagRect = (RectTransform)tagObject.transform;
            tagRect.SetParent(cardRect, false);
            tagRect.anchorMin = Vector2.zero;
            tagRect.anchorMax = Vector2.one;
            tagRect.offsetMin = new Vector2(10f, 8f);
            tagRect.offsetMax = new Vector2(-10f, -36f);

            FlowItemCardView card = cardObject.GetComponent<FlowItemCardView>();
            card.background = cardObject.GetComponent<Image>();
            card.titleText = titleObject.GetComponent<TextMeshProUGUI>();
            card.tagText = tagObject.GetComponent<TextMeshProUGUI>();
            card.ConfigureDefaults();
            return card;
        }

        public void Initialize(FlowRuntimeStore store)
        {
            flowStore = store;
        }

        private void Awake()
        {
            if (background == null)
            {
                background = GetComponent<Image>();
            }

            ConfigureDefaults();
        }

        public void Bind(FlowItem item)
        {
            if (item == null || item.Definition == null)
            {
                return;
            }

            this.item = item;

            if (background != null)
            {
                background.color = item.Definition.Color;
            }

            if (titleText != null)
            {
                titleText.text = item.Definition.DisplayName;
            }

            if (tagText != null)
            {
                tagText.text = string.Join(", ", item.Tags.Take(4));
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || flowStore == null || item == null)
            {
                return;
            }

            flowStore.SelectItem(item.ItemId);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (item != null)
            {
                FlowDragPayload.Begin(FlowDragPayload.Item, item.ItemId);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            FlowDragPayload.Clear();
        }

        private void ConfigureDefaults()
        {
            if (background != null)
            {
                background.raycastTarget = true;
            }

            ConfigureText(titleText, 18f, FontStyles.Bold);
            ConfigureText(tagText, 12f, FontStyles.Normal);
        }

        private static void ConfigureText(TMP_Text text, float size, FontStyles style)
        {
            if (text == null)
            {
                return;
            }

            text.raycastTarget = false;
            FlowTextStyle.Apply(text);
            text.fontSize = size;
            text.fontStyle = style;
            text.color = Color.black;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.overflowMode = TextOverflowModes.Ellipsis;
        }
    }
}
