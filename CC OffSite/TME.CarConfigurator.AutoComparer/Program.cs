using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Repository.Objects;
using TMME.CarConfigurator;

namespace TME.CarConfigurator.AutoComparer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (!options.Countries.Any())
                    throw new Exception("No country codes passed to process");
                

                var countryCodes = Administration.MyContext.GetContext().Countries
                                                                        .Cast<Administration.Country>()
                                                                        .Select(country => country.Code)
                                                                        .ToList();

                var invalidCodes = options.Countries.Except(countryCodes).ToList();

                if (invalidCodes.Any())
                    throw new Exception(String.Format("Invalid country codes: {0}", String.Join(", ", invalidCodes)));


                var readerMode = options.DataSubset == PublicationDataSubset.Live ? ReaderMode.Marketing : ReaderMode.MarketingPreview;

                var modelFactoryFacade = new ModelFactoryFacade().WithServiceFacade(new S3ServiceFacade().WithConfigurationManager(new ;

                foreach (var countryCode in options.Countries)
                {
                    ProcessCountry(countryCode, options);
                }
            }
        }

        private static void ProcessCountry(string countryCode, Options options)
        {
            var languageCodes = Administration.MyContext.GetContext().Countries[countryCode].Languages.Select(language => language.Code).ToList();

            foreach (var languageCode in languageCodes)
            {
                ProcessLanguage(countryCode, languageCode, options);
            }
        }

        private static void ProcessLanguage(string countryCode, string languageCode, Options options)
        {
            var context = new Context()
            {
                Brand = options.Brand,
                Country = countryCode,
                Language = languageCode
            };

            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);
            var newModels = CarConfigurator.DI.Models.GetModels(context).ToList();

            var oldModels = TMME.CarConfigurator.Models.GetModels(oldContext)
                                                       .Cast<TMME.CarConfigurator.Model>()
                                                       .Select(x => new CarConfigurator.LegacyAdapter.Model(x))
                                                       .ToList();

        }
    }
}
