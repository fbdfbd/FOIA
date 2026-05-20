using System;

namespace FOIA.Presentation
{
    public interface IPresenter : IDisposable
    {
        void Initialize();
    }
}
