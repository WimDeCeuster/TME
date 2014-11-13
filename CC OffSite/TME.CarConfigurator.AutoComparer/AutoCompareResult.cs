using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Enums;

namespace TME.CarConfigurator.AutoComparer
{
    public class AutoCompareResult
    {
        public String Brand { get; private set; }
        public String Environment { get; private set; }
        public String Target { get; private set; }
        public PublicationDataSubset DataSubset { get; private set; }

        public IReadOnlyList<CountryCompareResult> CountryCompareResults { get; private set; }

        public int TotalMismatches { get; private set; }
        public int TotalMissing { get; private set; }
        public int TotalMisorders { get; private set; }
        public int TotalExceptions { get; private set; }
        public int TotalNotImplemented { get; private set; }
        public bool IsValid { get; private set; }

        public AutoCompareResult(String brand, String environment, String target, PublicationDataSubset dataSubset, IEnumerable<CountryCompareResult> countryCompareResults)
        {
            Brand = brand;
            Environment = environment;
            Target = target;
            DataSubset = dataSubset;
            CountryCompareResults = countryCompareResults.ToList();

            TotalMismatches = CountryCompareResults.Sum(result => result.TotalMismatches);
            TotalMissing = CountryCompareResults.Sum(result => result.TotalMissing);
            TotalMisorders = CountryCompareResults.Sum(result => result.TotalMisorders);
            TotalExceptions = CountryCompareResults.Sum(result => result.TotalExceptions);
            TotalNotImplemented = CountryCompareResults.Sum(result => result.TotalNotImplemented);

            IsValid = CountryCompareResults.All(countryCompareResult => countryCompareResult.IsValid);
        }
    }
}
