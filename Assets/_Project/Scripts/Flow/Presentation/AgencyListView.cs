using System.Collections.Generic;
using FOIA.Flow.Runtime;
using FOIA.Graph.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace FOIA.Flow.Presentation
{
    public sealed class AgencyListView : MonoBehaviour
    {
        [SerializeField] private AgencyRuntimeStore agencyStore;
        [SerializeField] private FoiaProcessSystem processSystem;
        [SerializeField] private RectTransform contentRoot;

        private readonly List<AgencyDropView> views = new();

        private void Awake()
        {
            if (agencyStore == null)
            {
                agencyStore = GraphSceneLookup.FindFirst<AgencyRuntimeStore>();
            }

            if (processSystem == null)
            {
                processSystem = GraphSceneLookup.FindFirst<FoiaProcessSystem>();
            }

            if (contentRoot == null)
            {
                contentRoot = (RectTransform)transform;
            }

            EnsureLayout();
        }

        private void OnEnable()
        {
            if (agencyStore != null)
            {
                agencyStore.AgenciesChanged += Refresh;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (agencyStore != null)
            {
                agencyStore.AgenciesChanged -= Refresh;
            }
        }

        private void Refresh()
        {
            if (agencyStore == null)
            {
                return;
            }

            for (int i = 0; i < agencyStore.Agencies.Count; i++)
            {
                AgencyDropView view = GetView(i);
                view.Initialize(processSystem);
                view.Bind(agencyStore.Agencies[i]);
                view.gameObject.SetActive(true);
            }

            for (int i = agencyStore.Agencies.Count; i < views.Count; i++)
            {
                views[i].gameObject.SetActive(false);
            }
        }

        private AgencyDropView GetView(int index)
        {
            while (views.Count <= index)
            {
                views.Add(AgencyDropView.CreateDefault(contentRoot));
            }

            return views[index];
        }

        private void EnsureLayout()
        {
            if (!contentRoot.TryGetComponent(out VerticalLayoutGroup layout))
            {
                layout = contentRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            }

            layout.spacing = 10f;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
        }
    }
}
