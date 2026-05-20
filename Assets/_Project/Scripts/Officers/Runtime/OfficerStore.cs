using System.Collections.Generic;
using R3;

namespace FOIA.Officers.Runtime
{
    public sealed class OfficerStore
    {
        private readonly List<OfficerRuntime> _officers = new();
        private readonly Subject<OfficerRuntime> _onOfficerAdded = new();
        private readonly Subject<OfficerRuntime> _onSelectedOfficerChanged = new();

        private OfficerRuntime _selectedOfficer;

        public IReadOnlyList<OfficerRuntime> Officers => _officers;
        public OfficerRuntime SelectedOfficer => _selectedOfficer;

        public Observable<OfficerRuntime> OnOfficerAdded => _onOfficerAdded;
        public Observable<OfficerRuntime> OnSelectedOfficerChanged => _onSelectedOfficerChanged;

        public void Add(OfficerRuntime officer)
        {
            _officers.Add(officer);
            _onOfficerAdded.OnNext(officer);

            if (_selectedOfficer == null)
            {
                Select(officer);
            }
        }

        public void Select(OfficerRuntime officer)
        {
            if (_selectedOfficer == officer)
            {
                return;
            }

            _selectedOfficer = officer;
            _onSelectedOfficerChanged.OnNext(_selectedOfficer);
        }
    }
}
