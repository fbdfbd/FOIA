using FOIA.Flow.Input;
using FOIA.Flow.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FOIA.Flow.Presentation
{
    public sealed class StaffCardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text tagText;
        [SerializeField] private Image stressFill;

        private StaffRuntime staff;

        public static StaffCardView CreateDefault(RectTransform parent)
        {
            GameObject root = new("StaffCard", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(StaffCardView));
            RectTransform rect = (RectTransform)root.transform;
            rect.SetParent(parent, false);
            rect.sizeDelta = new Vector2(120f, 78f);

            Image bg = root.GetComponent<Image>();
            bg.color = new Color(0.18f, 0.18f, 0.22f, 1f);

            TextMeshProUGUI name = CreateText("Name", rect, 18f, FontStyles.Bold, new Vector2(8f, -30f), new Vector2(-8f, -6f));
            TextMeshProUGUI tag = CreateText("Tag", rect, 12f, FontStyles.Normal, new Vector2(8f, -54f), new Vector2(-8f, -32f));

            GameObject bar = new("StressBar", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform barRect = (RectTransform)bar.transform;
            barRect.SetParent(rect, false);
            barRect.anchorMin = new Vector2(0f, 0f);
            barRect.anchorMax = new Vector2(1f, 0f);
            barRect.offsetMin = new Vector2(8f, 8f);
            barRect.offsetMax = new Vector2(-8f, 14f);
            bar.GetComponent<Image>().color = Color.black;

            GameObject fill = new("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform fillRect = (RectTransform)fill.transform;
            fillRect.SetParent(barRect, false);
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(0f, 1f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            fill.GetComponent<Image>().color = new Color(0.9f, 0.2f, 0.2f, 1f);

            StaffCardView view = root.GetComponent<StaffCardView>();
            view.nameText = name;
            view.tagText = tag;
            view.stressFill = fill.GetComponent<Image>();
            return view;
        }

        public void Bind(StaffRuntime runtime)
        {
            staff = runtime;

            if (runtime == null || runtime.Definition == null)
            {
                return;
            }

            nameText.text = runtime.Definition.DisplayName;
            tagText.text = runtime.Definition.TraitTag;

            RectTransform fillRect = (RectTransform)stressFill.transform;
            fillRect.anchorMax = new Vector2(runtime.Stress / 100f, 1f);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (staff != null && staff.Definition != null)
            {
                FlowDragPayload.Begin(FlowDragPayload.Staff, staff.Definition.StaffId);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            FlowDragPayload.Clear();
        }

        private static TextMeshProUGUI CreateText(string name, RectTransform parent, float size, FontStyles style, Vector2 offsetMin, Vector2 offsetMax)
        {
            GameObject textObject = new(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            RectTransform rect = (RectTransform)textObject.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;

            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            FlowTextStyle.Apply(text);
            text.fontSize = size;
            text.fontStyle = style;
            text.color = Color.white;
            text.raycastTarget = false;
            text.overflowMode = TextOverflowModes.Ellipsis;
            return text;
        }
    }
}
