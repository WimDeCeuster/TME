using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.AutoComparer
{
    public class LanguageCompareResult
    {
        public String Language { get; private set; }
        public IReadOnlyList<ModelCompareResult> ModelCompareResults { get; private set; }
        public IReadOnlyList<Guid> MissingOldModelIds { get; private set; }
        public IReadOnlyList<Guid> MissingNewModelIds { get; private set; }

        public int TotalMismatches { get; private set; }
        public int TotalMissing { get; private set; }
        public int TotalMisorders { get; private set; }
        public int TotalExceptions { get; private set; }
        public int TotalNotImplemented { get; private set; }
        public bool IsValid { get; private set; }

        public LanguageCompareResult(string language, IEnumerable<ModelCompareResult> modelCompareResults, IEnumerable<Guid> missingOldModelIds, IEnumerable<Guid> missingNewModelIds)
        {
            Language = language;
            ModelCompareResults = modelCompareResults.ToList();
            MissingOldModelIds = missingOldModelIds.ToList();
            MissingNewModelIds = missingNewModelIds.ToList();

            TotalMismatches = ModelCompareResults.Sum(result => result.Result.Mismatches.Count);
            TotalMissing = ModelCompareResults.Sum(result => result.Result.Missing.Count);
            TotalMisorders = ModelCompareResults.Sum(result => result.Result.Misorders.Count);
            TotalExceptions = ModelCompareResults.Sum(result => result.Result.Exceptions.Count);
            TotalNotImplemented = ModelCompareResults.Sum(result => result.Result.NotImplemented.Count);

            IsValid = TotalMismatches + TotalMissing + TotalMisorders + TotalExceptions + TotalNotImplemented + MissingOldModelIds.Count + MissingNewModelIds.Count == 0;
        }
    }
}
