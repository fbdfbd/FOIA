using System.Collections.Generic;

namespace OneMoreSpoon.App.Encyclopedia.Data
{
    public sealed class DishDetailData : EncyclopediaDetailData
    {
        public string InputSubstanceName { get; }
        public IReadOnlyList<string> ProcessSteps { get; }
        public IReadOnlyList<string> ByproductNames { get; }

        public DishDetailData(
            string title,
            string description,
            string inputSubstanceName,
            IReadOnlyList<string> processSteps,
            IReadOnlyList<string> byproductNames)
            : base(title, description)
        {
            InputSubstanceName = inputSubstanceName;
            ProcessSteps = processSteps;
            ByproductNames = byproductNames;
        }
    }
}
