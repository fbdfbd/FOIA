using OneMoreSpoon.Game.Systems;
using OneMoreSpoon.View.UI;
using UnityEngine;
using VContainer.Unity;

namespace OneMoreSpoon.Presenter
{
    public sealed class SubstanceGrantButtonBindingSystem : IStartable
    {
        private readonly SubstanceGrantService grantService;

        public SubstanceGrantButtonBindingSystem(SubstanceGrantService grantService)
        {
            this.grantService = grantService;
        }

        public void Start()
        {
            foreach (var view in Object.FindObjectsByType<SubstanceGrantButtonView>(FindObjectsSortMode.None))
                view.Initialize(grantService);
        }
    }
}
