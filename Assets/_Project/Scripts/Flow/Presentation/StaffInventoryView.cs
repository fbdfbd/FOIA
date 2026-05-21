using System.Collections.Generic;
using FOIA.Flow.Runtime;
using FOIA.Graph.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace FOIA.Flow.Presentation
{
    public sealed class StaffInventoryView : MonoBehaviour
    {
        [SerializeField] private StaffRuntimeStore staffStore;
        [SerializeField] private ProcessStateStore processState;
        [SerializeField] private RectTransform contentRoot;
        [SerializeField] private float staffNameFontSize = 20f;
        [SerializeField] private float staffTagFontSize = 14f;

        private readonly List<StaffCardView> cards = new();

        private void Awake()
        {
            if (staffStore == null)
            {
                staffStore = GraphSceneLookup.FindFirst<StaffRuntimeStore>();
            }

            if (processState == null)
            {
                processState = GraphSceneLookup.FindFirst<ProcessStateStore>();
            }

            if (contentRoot == null)
            {
                contentRoot = (RectTransform)transform;
            }

            EnsureLayout();
        }

        private void OnEnable()
        {
            if (staffStore != null)
            {
                staffStore.StaffChanged += Refresh;
            }

            if (processState != null)
            {
                processState.StateChanged += Refresh;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (staffStore != null)
            {
                staffStore.StaffChanged -= Refresh;
            }

            if (processState != null)
            {
                processState.StateChanged -= Refresh;
            }
        }

        private void Refresh()
        {
            if (staffStore == null)
            {
                return;
            }

            int index = 0;

            foreach (StaffRuntime staff in staffStore.Staff)
            {
                bool isEquipped = processState != null && staff.Definition.StaffId == processState.EquippedStaffId;

                if (!staff.IsActive || isEquipped)
                {
                    continue;
                }

                StaffCardView card = GetCard(index);
                card.ConfigureFontSizes(staffNameFontSize, staffTagFontSize);
                card.Bind(staff);
                card.gameObject.SetActive(true);
                index++;
            }

            for (int i = index; i < cards.Count; i++)
            {
                cards[i].gameObject.SetActive(false);
            }
        }

        private StaffCardView GetCard(int index)
        {
            while (cards.Count <= index)
            {
                cards.Add(StaffCardView.CreateDefault(contentRoot));
            }

            return cards[index];
        }

        private void EnsureLayout()
        {
            if (!contentRoot.TryGetComponent(out HorizontalLayoutGroup layout))
            {
                layout = contentRoot.gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            layout.spacing = 10f;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
        }
    }
}
