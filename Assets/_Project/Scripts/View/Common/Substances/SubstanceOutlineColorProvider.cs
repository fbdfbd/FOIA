using OneMoreSpoon.Game.Definitions;

namespace OneMoreSpoon.View.Substances
{
    public sealed class SubstanceOutlineColorProvider
    {
        private readonly SO_SubstanceOutlineConfig config;

        public SubstanceOutlineColorProvider(SO_SubstanceOutlineConfig config)
        {
            this.config = config;
        }

        public SubstanceOutlineColors GetColors(SubstanceKind kind)
        {
            return config != null
                ? config.GetColors(kind)
                : SubstanceOutlineColors.Default;
        }
    }
}
