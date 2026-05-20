using System.Collections.Generic;
using FOIA.Core;

namespace FOIA.Runtime
{
    public readonly struct ProcessResult
    {
        public ProcessResult(ResultGrade grade, IReadOnlyDictionary<ProcessTag, int> tags, IReadOnlyDictionary<ByproductType, int> byproducts, IReadOnlyList<ProcessEffect> effects)
        {
            Grade = grade;
            Tags = tags;
            Byproducts = byproducts;
            Effects = effects;
        }

        public ResultGrade Grade { get; }
        public IReadOnlyDictionary<ProcessTag, int> Tags { get; }
        public IReadOnlyDictionary<ByproductType, int> Byproducts { get; }
        public IReadOnlyList<ProcessEffect> Effects { get; }
    }
}
