using FOIA.Flow.Input;
using FOIA.Flow.Runtime;
using FOIA.Graph.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FOIA.Flow.Presentation
{
    public sealed class AgencyDropView : MonoBehaviour, IDropHandler
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text traitText;
        [SerializeField] private Image relationshipFill;

        private FoiaProcessSystem processSystem;
        private AgencyRuntime agency;

        public static AgencyDropView CreateDefault(RectTransform parent)
        {
            GameObject root = new("AgencySlot", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(AgencyDropView));
            RectTransform rect = (RectTransform)root.transform;
            rect.SetParent(parent, false);
            rect.sizeDelta = new Vector2(170f, 86f);
            root.GetComponent<Image>().color = new Color(0.14f, 0.14f, 0.16f, 1f);

            TextMeshProUGUI name = CreateText("Name", rect, 17f, FontStyles.Bold, new Vector2(8f, -30f), new Vector2(-8f, -6f));
            TextMeshProUGUI trait = CreateText("Trait", rect, 12f, FontStyles.Normal, new Vector2(8f, -54f), new Vector2(-8f, -32f));

            GameObject fillObject = new("Relationship", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform fillRect = (RectTransform)fillObject.transform;
            fillRect.SetParent(rect, false);
            fillRect.anchorMin = new Vector2(0f, 0f);
            fillRect.anchorMax = new Vector2(0f, 0f);
            fillRect.offsetMin = new Vector2(8f, 8f);
            fillRect.offsetMax = new Vector2(8f, 14f);
            fillObject.GetComponent<Image>().color = new Color(0.1f, 0.8f, 0.3f, 1f);

            AgencyDropView view = root.GetComponent<AgencyDropView>();
            view.nameText = name;
            view.traitText = trait;
            view.relationshipFill = fillObject.GetComponent<Image>();
            return view;
        }

        public void Initialize(FoiaProcessSystem system)
        {
            processSystem = system;
        }

        public void Bind(AgencyRuntime runtime)
        {
            agency = runtime;
            nameText.text = runtime.Definition.DisplayName;
            traitText.text = runtime.Definition.TraitTag;
            RectTransform rect = (RectTransform)relationshipFill.transform;
            rect.anchorMax = new Vector2(runtime.Relationship / 100f, 0f);
            rect.offsetMax = new Vector2(8f + 154f * runtime.Relationship / 100f, 14f);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (agency != null && FlowDragPayload.Is(FlowDragPayload.Document))
            {
                processSystem.ProcessCurrentDocumentAtAgency(agency.Definition.AgencyId);
            }
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
