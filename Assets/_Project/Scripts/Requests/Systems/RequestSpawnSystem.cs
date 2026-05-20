using System;
using FOIA.GameLoop.State;
using FOIA.Requests.Definitions;
using FOIA.Requests.Runtime;
using FOIA.Requests.State;

namespace FOIA.Requests.Systems
{
    public sealed class RequestSpawnSystem
    {
        private int _nextCaseNumber = 1;

        private readonly GameCalendarState _calendarState;
        private readonly RequestCaseStore _caseStore;

        public RequestSpawnSystem(
            GameCalendarState calendarState,
            RequestCaseStore caseStore)
        {
            _calendarState = calendarState;
            _caseStore = caseStore;
        }

        public RequestCaseRuntime SpawnRequest(SO_RequestDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            RequestCaseRuntime requestCase = new(
                CreateCaseId(),
                definition,
                _calendarState.CurrentDay,
                _calendarState.CurrentHour);

            _caseStore.Add(requestCase);
            return requestCase;
        }

        private string CreateCaseId()
        {
            return $"case_{_nextCaseNumber++:000}";
        }
    }
}
