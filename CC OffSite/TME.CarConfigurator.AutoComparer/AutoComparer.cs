using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.QueryServices.Exceptions;
using TMME.CarConfigurator;
using TMME.CarConfigurator.Exceptions;

namespace TME.CarConfigurator.AutoComparer
{
    public class AutoComparer : IAutoComparer
    {
        public AutoCompareResult Compare(IEnumerable<string> countries, String brand, PublicationDataSubset dataSubset)
        {
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand cannot be empty");

            Administration.MyContext.SetSystemContext(brand, "ZZ", "EN");
            var countryCodes = Administration.MyContext.GetContext().Countries
                                                                    .Cast<Administration.Country>()
                                                                    .Select(country => country.Code)
                                                                    .ToList();

            var invalidCodes = countries.Except(countryCodes).ToList();

            if (invalidCodes.Any())
                throw new Exception(String.Format("Invalid country codes: {0}", String.Join(", ", invalidCodes)));

            var readerMode = dataSubset == PublicationDataSubset.Live ? ReaderMode.Marketing : ReaderMode.MarketingPreview;

            return new AutoCompareResult(brand, dataSubset, countries.Select(countryCode => ProcessCountry(countryCode, brand)));
        }

        private CountryCompareResult ProcessCountry(string countryCode, string brand)
        {
            Console.WriteLine("Processing country {0}", countryCode);

            var languageCodes = Administration.MyContext.GetContext().Countries[countryCode].Languages.Select(language => language.Code).ToList();

            var results = new List<LanguageCompareResult>();
            var missingLanguages = new List<String>();

            foreach (var languageCode in languageCodes)
            {
                try
                {
                    results.Add(ProcessLanguage(countryCode, languageCode, brand));
                }
                catch (CountryLanguageCombinationDoesNotExistException)
                {
                    missingLanguages.Add(languageCode);
                }
            }

            return new CountryCompareResult(countryCode, results, missingLanguages);
        }

        private LanguageCompareResult ProcessLanguage(string countryCode, string languageCode, string brand)
        {
            Console.WriteLine("Processing language {0}-{1}", countryCode, languageCode);

            var context = new Context()
            {
                Brand = brand,
                Country = countryCode,
                Language = languageCode
            };

            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);
            var newModels = CarConfigurator.DI.Models.GetModels(context).ToList();

            var oldModels = TMME.CarConfigurator.Models.GetModels(oldContext)
                                                       .Cast<TMME.CarConfigurator.Model>()
                                                       .Select(x => new CarConfigurator.LegacyAdapter.Model(x))
                                                       .ToList();

            var newModelIds = newModels.Select(model => model.ID).ToList();
            var oldModelIds = oldModels.Select(model => model.ID).ToList();

            var missingOldModelIds = newModelIds.Except(oldModelIds).ToList();
            var missingNewModelIds = oldModelIds.Except(newModelIds).ToList();

            //var missingOldModels = oldModels.Where(model => missingOldModelIds.Contains(model.ID)).ToList();
            //var missingNewModels = newModels.Where(model => missingNewModelIds.Contains(model.ID)).ToList();

            var presentModelIds = newModelIds.Intersect(oldModelIds).ToList();

            var results = presentModelIds.Take(1).AsParallel().Select(modelId =>
            {
                try
                {
                    var x = MyContext.CurrentContext;
                }
                catch (CurrentContextEmpty)
                {
                    MyContext.NewContext(context.Brand, context.Country, context.Language);
                }

                var threadModels = TMME.CarConfigurator.Models.GetModels(oldContext)
                                                              .Cast<TMME.CarConfigurator.Model>()
                                                              .Select(x => new CarConfigurator.LegacyAdapter.Model(x))
                                                              .ToList();

                var oldModel = threadModels.Single(model => model.ID == modelId);
                var newModel = newModels.Single(model => model.ID == modelId);

                Console.WriteLine("Processing model {0}-{1}-{2}", countryCode, languageCode, oldModel.Name);

                var comparisonResult = new Comparer.Comparer().Compare(oldModel, newModel);

                return new ModelCompareResult(oldModel.Name, comparisonResult);
            }).ToList();

            return new LanguageCompareResult(languageCode, results, missingNewModelIds, missingNewModelIds);
        }
    }
}
