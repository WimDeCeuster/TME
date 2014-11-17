using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.AutoComparer
{
    public class CountryCompareResult
    {
        public string Country { get; private set; }
        public IReadOnlyList<LanguageCompareResult> LanguageCompareResults { get; private set; }

        public IReadOnlyList<Guid> MissingOldModelIds { get; private set; }
        public IReadOnlyList<Guid> MissingNewModelIds { get; private set; }
        public IReadOnlyList<String> MissingLanguages { get; private set; }

        public int TotalMismatches { get; private set; }
        public int TotalMissing { get; private set; }
        public int TotalMisorders { get; private set; }
        public int TotalExceptions { get; private set; }
        public int TotalNotImplemented { get; private set; }
        public bool IsValid { get; private set; }

        public CountryCompareResult(String country, IEnumerable<LanguageCompareResult> languageCompareResults, IEnumerable<String> missingLanguages)
        {
            Country = country;
            LanguageCompareResults = languageCompareResults.ToList();
            MissingLanguages = missingLanguages.ToList();

            MissingOldModelIds = LanguageCompareResults.SelectMany(result => result.MissingOldModelIds).Distinct().ToList();
            MissingNewModelIds = LanguageCompareResults.SelectMany(result => result.MissingNewModelIds).Distinct().ToList();

            TotalMismatches = LanguageCompareResults.Sum(result => result.TotalMismatches);
            TotalMissing = LanguageCompareResults.Sum(result => result.TotalMissing);
            TotalMisorders = LanguageCompareResults.Sum(result => result.TotalMisorders);
            TotalExceptions = LanguageCompareResults.Sum(result => result.TotalExceptions);
            TotalNotImplemented = LanguageCompareResults.Sum(result => result.TotalNotImplemented);

            IsValid = LanguageCompareResults.All(languageCompareResult => languageCompareResult.IsValid) && !MissingLanguages.Any();
        }


    }
}
