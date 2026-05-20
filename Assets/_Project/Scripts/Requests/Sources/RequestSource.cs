using System;
using System.Collections.Generic;
using FOIA.Requests.Definitions;
using R3;

namespace FOIA.Requests.Sources
{
    public sealed class RequestSource
    {
        private readonly List<SO_RequestDefinition> _definitions = new();
        private readonly Subject<SO_RequestDefinition> _onRequestAdded = new();
        private int _nextIndex;

        public IReadOnlyList<SO_RequestDefinition> Definitions => _definitions;
        public bool HasRequest => _definitions.Count > 0;
        public Observable<SO_RequestDefinition> OnRequestAdded => _onRequestAdded;

        public void Add(SO_RequestDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            _definitions.Add(definition);
            _onRequestAdded.OnNext(definition);
        }

        public void AddRange(IEnumerable<SO_RequestDefinition> definitions)
        {
            if (definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            foreach (SO_RequestDefinition definition in definitions)
            {
                Add(definition);
            }
        }

        public SO_RequestDefinition GetNextRequest()
        {
            if (!HasRequest)
            {
                throw new InvalidOperationException($"{nameof(RequestSource)} has no request definitions.");
            }

            SO_RequestDefinition definition = _definitions[_nextIndex];
            _nextIndex = (_nextIndex + 1) % _definitions.Count;
            return definition;
        }

        public SO_RequestDefinition PeekNextRequest()
        {
            if (!HasRequest)
            {
                throw new InvalidOperationException($"{nameof(RequestSource)} has no request definitions.");
            }

            return _definitions[_nextIndex];
        }
    }
}
