using FOIA.Definitions;

namespace FOIA.Runtime
{
    public sealed class CraftingService
    {
        private readonly ByproductWallet wallet;
        private readonly ProcessBoardState boardState;

        public CraftingService(ByproductWallet wallet, ProcessBoardState boardState)
        {
            this.wallet = wallet;
            this.boardState = boardState;
        }

        public bool CanCraft(CraftingRecipeDefinition recipe)
        {
            return recipe != null
                && recipe.OutputBlock != null
                && wallet.CanSpend(recipe.Costs);
        }

        public bool TryCraft(CraftingRecipeDefinition recipe)
        {
            if (!CanCraft(recipe) || !wallet.TrySpend(recipe.Costs))
                return false;

            boardState.AddBlock(recipe.OutputBlock.Id, recipe.OutputAmount);
            return true;
        }
    }
}
