using System.Collections.Generic;

namespace OneMoreSpoon.App.Encyclopedia.Data
{
    public sealed class EdgeBlockDetailData : EncyclopediaDetailData
    {
        public IReadOnlyList<string> InputSubstanceNames { get; }

        public EdgeBlockDetailData(
            string title,
            string description,
            IReadOnlyList<string> inputSubstanceNames)
            : base(title, description)
        {
            InputSubstanceNames = inputSubstanceNames;
        }
    }
}
