using System;
using System.Collections.Generic;
using System.Linq;
using VContainer.Unity;

namespace FOIA.Presentation
{
    public sealed class PresenterInitializer : IStartable, IDisposable
    {
        private readonly IPresenter[] _presenters;

        public PresenterInitializer(IEnumerable<IPresenter> presenters)
        {
            _presenters = presenters.ToArray();
        }

        public void Start()
        {
            foreach (IPresenter presenter in _presenters)
            {
                presenter.Initialize();
            }
        }

        public void Dispose()
        {
            foreach (IPresenter presenter in _presenters)
            {
                presenter.Dispose();
            }
        }
    }
}
