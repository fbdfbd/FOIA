using FOIA.Officers.Definitions;
using FOIA.Officers.Runtime;
using FOIA.Requests.Definitions;
using FOIA.Requests.Sources;
using UnityEngine;
using VContainer;

namespace FOIA.App
{
    public sealed class RequestBoardTestBootstrapper : MonoBehaviour
    {
        [SerializeField] private SO_OfficerDefinition[] _officerDefinitions;
        [SerializeField] private SO_RequestDefinition[] _requestDefinitions;

        private OfficerStore _officerStore;
        private RequestSource _requestSource;

        [Inject]
        public void Construct(OfficerStore officerStore, RequestSource requestSource)
        {
            _officerStore = officerStore;
            _requestSource = requestSource;
        }

        private void Start()
        {
            AddOfficers();
            AddRequests();
        }

        private void AddOfficers()
        {
            for (int i = 0; i < _officerDefinitions.Length; i++)
            {
                SO_OfficerDefinition definition = _officerDefinitions[i];

                if (definition == null)
                {
                    continue;
                }

                _officerStore.Add(new OfficerRuntime($"officer_{i + 1:000}", definition));
            }
        }

        private void AddRequests()
        {
            foreach (SO_RequestDefinition definition in _requestDefinitions)
            {
                if (definition == null)
                {
                    continue;
                }

                _requestSource.Add(definition);
            }
        }
    }
}
