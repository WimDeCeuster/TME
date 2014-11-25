using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.QueryServices.Exceptions;
using TMME.CarConfigurator;
using TMME.CarConfigurator.Exceptions;

namespace TME.CarConfigurator.AutoComparer
{
    public class AutoComparer : IAutoComparer
    {
        public AutoCompareResult Compare(IList<string> countries, String brand, PublicationDataSubset dataSubset)
        {
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");

            Administration.MyContext.SetSystemContext(brand, "ZZ", "EN");
            var countryCodes = Administration.MyContext.GetContext().Countries
                                                                    .Select(country => country.Code)
                                                                    .ToList();

            var invalidCodes = countries.Except(countryCodes).ToList();

            if (invalidCodes.Any())
                throw new Exception(String.Format("Invalid country codes: {0}", String.Join(", ", invalidCodes)));

            var readerMode = dataSubset == PublicationDataSubset.Live ? ReaderMode.Marketing : ReaderMode.MarketingPreview;

            return new AutoCompareResult(brand, dataSubset, countries.Select(countryCode => ProcessCountry(countryCode, brand, readerMode)));
        }

        private CountryCompareResult ProcessCountry(string countryCode, string brand, ReaderMode readerMode)
        {
            Console.WriteLine("Processing country {0}", countryCode);

            var languageCodes = Administration.MyContext.GetContext().Countries[countryCode].Languages.Select(language => language.Code).ToList();

            var results = new List<LanguageCompareResult>();
            var missingLanguages = new List<String>();

            foreach (var languageCode in languageCodes)
            {
                try
                {
                    results.Add(ProcessLanguage(countryCode, languageCode, brand, readerMode));
                }
                catch (CountryLanguageCombinationDoesNotExistException)
                {
                    missingLanguages.Add(languageCode);
                }
            }

            return new CountryCompareResult(countryCode, results, missingLanguages);
        }

        private LanguageCompareResult ProcessLanguage(string countryCode, string languageCode, string brand, ReaderMode readerMode)
        {
            Console.WriteLine("Processing language {0}-{1}", countryCode, languageCode);

            var context = new Context()
            {
                Brand = brand,
                Country = countryCode,
                Language = languageCode
            };

            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language, readerMode);
            var newModels = DI.Models.GetModels(context).ToList();

            var oldModels = Models.GetModels(oldContext)
                                                       .Cast<TMME.CarConfigurator.Model>()
                                                       .Select(x => new LegacyAdapter.Model(x))
                                                       .ToList();

            var newModelIds = newModels.Select(model => model.ID).ToList();
            var oldModelIds = oldModels.Select(model => model.ID).ToList();

            var missingNewModelIds = oldModelIds.Except(newModelIds).ToList();

            var presentModelIds = newModelIds.Intersect(oldModelIds).ToList();

            var results = presentModelIds.AsParallel().Select(modelId =>
            {
                try
                {
                    var x = MyContext.CurrentContext;
                }
                catch (CurrentContextEmpty)
                {
                    MyContext.NewContext(context.Brand, context.Country, context.Language, readerMode);
                }

                var threadModels = Models.GetModels(oldContext)
                                                              .Cast<TMME.CarConfigurator.Model>()
                                                              .Select(x => new LegacyAdapter.Model(x))
                                                              .ToList();

                var oldModel = threadModels.Single(model => model.ID == modelId);
                var newModel = newModels.Single(model => model.ID == modelId);

                var start = DateTime.Now;

                Console.WriteLine("Processing model {0}-{1}-{2}", countryCode, languageCode, oldModel.Name);

                var comparisonResult = new Comparer.Comparer().Compare(oldModel, newModel);
                
                var result =  new ModelCompareResult(oldModel.Name, comparisonResult);

                Console.WriteLine("Done processing model {0}-{1}-{2} in {3} seconds", countryCode, languageCode, oldModel.Name, DateTime.Now.Subtract(start).TotalMilliseconds/1000);

                return result;
            }).ToList();

            return new LanguageCompareResult(languageCode, results, missingNewModelIds, missingNewModelIds);
        }
    }
}
