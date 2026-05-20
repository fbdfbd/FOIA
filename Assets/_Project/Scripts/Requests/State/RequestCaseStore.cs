using System.Collections.Generic;
using FOIA.Requests.Runtime;
using R3;

namespace FOIA.Requests.State
{
    public sealed class RequestCaseStore
    {
        private readonly List<RequestCaseRuntime> _cases = new();
        private readonly Subject<RequestCaseRuntime> _onCaseAdded = new();
        private readonly Subject<RequestCaseRuntime> _onSelectedCaseChanged = new();
        private RequestCaseRuntime _selectedCase;

        public IReadOnlyList<RequestCaseRuntime> Cases => _cases;
        public RequestCaseRuntime SelectedCase => _selectedCase;

        public Observable<RequestCaseRuntime> OnCaseAdded => _onCaseAdded;
        public Observable<RequestCaseRuntime> OnSelectedCaseChanged => _onSelectedCaseChanged;

        public void Add(RequestCaseRuntime requestCase)
        {
            _cases.Add(requestCase);
            _onCaseAdded.OnNext(requestCase);
            Select(requestCase);
        }

        public void Select(RequestCaseRuntime requestCase)
        {
            if (_selectedCase == requestCase)
            {
                return;
            }

            _selectedCase = requestCase;
            _onSelectedCaseChanged.OnNext(_selectedCase);
        }
    }
}
