using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.AutoComparer.Extensions;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.QueryServices.Exceptions;
using TMME.CarConfigurator;
using TMME.CarConfigurator.Exceptions;

namespace TME.CarConfigurator.AutoComparer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                var countries = options.Countries;
                if (!countries.Any())
                {
                    countries = (ConfigurationManager.AppSettings["Countries"] ?? "").Split(' ').ToList();

                    if (!countries.Any())
                        throw new Exception("No country codes passed to process");
                }

                Administration.MyContext.SetSystemContext(options.Brand, "ZZ", "EN");
                var countryCodes = Administration.MyContext.GetContext().Countries
                                                                        .Cast<Administration.Country>()
                                                                        .Select(country => country.Code)
                                                                        .ToList();

                var invalidCodes = countries.Except(countryCodes).ToList();

                if (invalidCodes.Any())
                    throw new Exception(String.Format("Invalid country codes: {0}", String.Join(", ", invalidCodes)));

                var readerMode = options.DataSubset == PublicationDataSubset.Live ? ReaderMode.Marketing : ReaderMode.MarketingPreview;

                var result = new AutoCompareResult(options.Brand, options.Environment, options.Target, options.DataSubset, countries.Select(countryCode => ProcessCountry(countryCode, options)));

                WriteReport(result);
            }
        }

        private static void WriteReport(AutoCompareResult result)
        {
            Directory.Delete("Report", true);

            var output = new StringBuilder();

            output.AppendLine("AutoCompare result:");
            output.AppendLine();
            var countries = String.Join(", ", result.CountryCompareResults.Select(countryCompareResult => countryCompareResult.Country));
            output.AppendLine("Brand: {0} | DataSubset: {1} | Countries ({2}): {3}", result.Brand, result.DataSubset, result.CountryCompareResults.Count, countries);
            output.AppendLine("Valid: {0} | Total mismatches: {1} | Total missing: {2} | Total misorders: {3} | Total exceptions: {4} | Total not implemented: {5}",
                                result.IsValid, result.TotalMismatches, result.TotalMissing, result.TotalMisorders, result.TotalExceptions, result.TotalNotImplemented);

            output.AppendLine("---------------------------------------");
            output.AppendLine();

            foreach (var countryCompareResult in result.CountryCompareResults)
            {
                WriteCountryReport(countryCompareResult, output);
                output.AppendLine();
            }

            File.WriteAllText(Path.Combine("Report", "report.txt"), output.ToString());
        }

        private static void WriteCountryReport(CountryCompareResult result, StringBuilder output)
        {
            var countryOutput = new StringBuilder();

            countryOutput.AppendLine("Result for {0}:", result.Country);
            countryOutput.AppendLine();
            var languages = String.Join(", ", result.LanguageCompareResults.Select(languageCompareResult => languageCompareResult.Language));
            countryOutput.AppendLine("Valid: {0} | Total mismatches: {1} | Total missing: {2} | Total misorders: {3} | Total exceptions: {4} | Total not implemented: {5}",
                                result.IsValid, result.TotalMismatches, result.TotalMissing, result.TotalMisorders, result.TotalExceptions, result.TotalNotImplemented);

            if (result.MissingLanguages.Any())
                countryOutput.AppendLine("Missing languages: {0}", String.Join(", ", result.MissingLanguages));
            if (result.MissingNewModelIds.Any())
                countryOutput.AppendLine("Missing new models: {0}", String.Join(", ", result.MissingNewModelIds.Select(id => id.ToString())));
            if (result.MissingOldModelIds.Any())
                countryOutput.AppendLine("Missing old models: {0}", String.Join(", ", result.MissingNewModelIds.Select(id => id.ToString())));
            countryOutput.AppendLine("---------------------------------------");

            output.Append(countryOutput.ToString());
            
            countryOutput.AppendLine();

            foreach (var languageCompareResult in result.LanguageCompareResults)
            {
                WriteLanguageReport(languageCompareResult, result.Country, countryOutput);
                countryOutput.AppendLine();
            }

            var dir = Path.Combine("Report", result.Country);
            File.WriteAllText(Path.Combine(dir, String.Format("report-{0}.txt", result.Country)), countryOutput.ToString());
        }

        private static void WriteLanguageReport(LanguageCompareResult result, string country, StringBuilder output)
        {
            var languageOutput = new StringBuilder();

            languageOutput.AppendLine("Result for {0}-{1}:", country, result.Language);
            languageOutput.AppendLine();

            languageOutput.AppendLine("Valid: {0} | Total mismatches: {1} | Total missing: {2} | Total misorders: {3} | Total exceptions: {4} | Total not implemented: {5}",
                                result.IsValid, result.TotalMismatches, result.TotalMissing, result.TotalMisorders, result.TotalExceptions, result.TotalNotImplemented);

            if(result.MissingNewModelIds.Any())
                languageOutput.AppendLine("Missing new models: {0}", String.Join(", ", result.MissingNewModelIds.Select(id => id.ToString())));
            if(result.MissingOldModelIds.Any())
                languageOutput.AppendLine("Missing old models: {0}", String.Join(", ", result.MissingNewModelIds.Select(id => id.ToString())));
            languageOutput.AppendLine("---------------------------------------");

            output.Append(languageOutput.ToString());
            
            languageOutput.AppendLine();

            foreach (var modelCompareResult in result.ModelCompareResults)
            {
                WriteModelReport(modelCompareResult, country, result.Language, languageOutput);
                languageOutput.AppendLine();
            }

            var dir = Path.Combine("Report", country, result.Language);
            File.WriteAllText(Path.Combine(dir, String.Format("report-{0}-{1}.txt", country, result.Language)), languageOutput.ToString());
        }

        private static void WriteModelReport(ModelCompareResult result, string country, string language, StringBuilder xoutput)
        {
            var modelOutput = new StringBuilder();

            modelOutput.AppendLine("Result for {0}-{1} {2}:", country, language, result.ModelName);
            modelOutput.AppendLine();

            modelOutput.AppendLine("Valid: {0} | Total mismatches: {1} | Total missing: {2} | Total misorders: {3} | Total exceptions: {4} | Total not implemented: {5}",
                                result.Result.IsValid, result.Result.Mismatches.Count, result.Result.Missing.Count, result.Result.Misorders.Count, result.Result.Exceptions.Count, result.Result.NotImplemented.Count);
            modelOutput.AppendLine("---------------------------------------");

            modelOutput.Append(modelOutput.ToString());

            modelOutput.AppendLine();

            if (result.Result.Mismatches.Any())
            {
                modelOutput.AppendLine("Mismatches ({0}):", result.Result.Mismatches.Count);
                foreach (var item in result.Result.Mismatches)
                    modelOutput.AppendLine(item.ToString());
                modelOutput.AppendLine();
            }

            if (result.Result.Missing.Any())
            {
                modelOutput.AppendLine("Missing ({0}):", result.Result.Missing.Count);
                foreach (var item in result.Result.Missing)
                    modelOutput.AppendLine(item.ToString());
                modelOutput.AppendLine();
            }

            if (result.Result.Misorders.Any())
            {
                modelOutput.AppendLine("Misorders ({0}):", result.Result.Misorders.Count);
                foreach (var item in result.Result.Misorders)
                    modelOutput.AppendLine(item.ToString());
                modelOutput.AppendLine();
            }

            if (result.Result.Exceptions.Any())
            {
                modelOutput.AppendLine("Exceptions ({0}):", result.Result.Exceptions.Count);
                foreach (var item in result.Result.Missing)
                    modelOutput.AppendLine(item.ToString());
                modelOutput.AppendLine();
            }

            if (result.Result.NotImplemented.Any())
            {
                modelOutput.AppendLine("Not implemented ({0}):", result.Result.NotImplemented.Count);
                foreach (var item in result.Result.NotImplemented)
                    modelOutput.AppendLine(item.ToString());
                modelOutput.AppendLine();
            }

            var dir = Path.Combine("Report", country, language);
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, String.Format("report-{0}-{1}-{2}.txt", country, language, result.ModelName)), modelOutput.ToString());
        }

        private static CountryCompareResult ProcessCountry(string countryCode, Options options)
        {
            Console.WriteLine("Processing country {0}", countryCode);

            var languageCodes = Administration.MyContext.GetContext().Countries[countryCode].Languages.Select(language => language.Code).ToList();

            var results = new List<LanguageCompareResult>();
            var missingLanguages = new List<String>();

            foreach (var languageCode in languageCodes)
            {
                try
                {
                    results.Add(ProcessLanguage(countryCode, languageCode, options));
                }
                catch (CountryLanguageCombinationDoesNotExistException)
                {
                    missingLanguages.Add(languageCode);
                }
            }

            return new CountryCompareResult(countryCode, results, missingLanguages);
        }

        private static LanguageCompareResult ProcessLanguage(string countryCode, string languageCode, Options options)
        {
            Console.WriteLine("Processing language {0}-{1}", countryCode, languageCode);

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

            var newModelIds = newModels.Select(model => model.ID).ToList();
            var oldModelIds = oldModels.Select(model => model.ID).ToList();

            var missingOldModelIds = newModelIds.Except(oldModelIds).ToList();
            var missingNewModelIds = oldModelIds.Except(newModelIds).ToList();

            //var missingOldModels = oldModels.Where(model => missingOldModelIds.Contains(model.ID)).ToList();
            //var missingNewModels = newModels.Where(model => missingNewModelIds.Contains(model.ID)).ToList();

            var presentModelIds = newModelIds.Intersect(oldModelIds).ToList();

            var results = presentModelIds.AsParallel().Select(modelId =>
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
