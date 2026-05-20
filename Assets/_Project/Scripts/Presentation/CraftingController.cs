using FOIA.Definitions;
using FOIA.Runtime;
using UnityEngine;
using VContainer;

namespace FOIA.Presentation
{
    public sealed class CraftingController : MonoBehaviour
    {
        private CraftingService craftingService;

        [Inject]
        public void Construct(CraftingService craftingService)
        {
            this.craftingService = craftingService;
        }

        public void Craft(CraftingRecipeDefinition recipe)
        {
            craftingService?.TryCraft(recipe);
        }
    }
}
