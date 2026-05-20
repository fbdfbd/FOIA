using R3;

namespace FOIA.Presentation
{
    public abstract class PresenterBase : IPresenter
    {
        protected readonly CompositeDisposable Disposables = new();

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            OnInitialize();
            IsInitialized = true;
        }

        protected abstract void OnInitialize();

        public virtual void Dispose()
        {
            Disposables.Dispose();
        }
    }
}
