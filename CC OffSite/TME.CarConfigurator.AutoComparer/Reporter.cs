using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.AutoComparer.Extensions;

namespace TME.CarConfigurator.AutoComparer
{
    class Reporter : IReporter
    {
        private const string DirName = "Report";

        public void WriteReport(AutoCompareResult result, string path = "")
        {
            path = path ?? "";

            if (Directory.Exists(DirName)) Directory.Delete(DirName, true);

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
                WriteCountryReport(countryCompareResult, output, path);
                output.AppendLine();
            }

            File.WriteAllText(Path.Combine(path, DirName, "report.txt"), output.ToString());
        }

        private void WriteCountryReport(CountryCompareResult result, StringBuilder output, string path)
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
                WriteLanguageReport(languageCompareResult, result.Country, countryOutput, path);
                countryOutput.AppendLine();
            }

            var dir = Path.Combine(path, DirName, result.Country);
            File.WriteAllText(Path.Combine(dir, String.Format("report-{0}.txt", result.Country)), countryOutput.ToString());
        }

        private void WriteLanguageReport(LanguageCompareResult result, string country, StringBuilder output, string path)
        {
            var languageOutput = new StringBuilder();

            languageOutput.AppendLine("Result for {0}-{1}:", country, result.Language);
            languageOutput.AppendLine();

            languageOutput.AppendLine("Valid: {0} | Total mismatches: {1} | Total missing: {2} | Total misorders: {3} | Total exceptions: {4} | Total not implemented: {5}",
                                result.IsValid, result.TotalMismatches, result.TotalMissing, result.TotalMisorders, result.TotalExceptions, result.TotalNotImplemented);

            if (result.MissingNewModelIds.Any())
                languageOutput.AppendLine("Missing new models: {0}", String.Join(", ", result.MissingNewModelIds.Select(id => id.ToString())));
            if (result.MissingOldModelIds.Any())
                languageOutput.AppendLine("Missing old models: {0}", String.Join(", ", result.MissingNewModelIds.Select(id => id.ToString())));
            languageOutput.AppendLine("---------------------------------------");

            output.Append(languageOutput.ToString());

            languageOutput.AppendLine();

            foreach (var modelCompareResult in result.ModelCompareResults)
            {
                WriteModelReport(modelCompareResult, country, result.Language, languageOutput, path);
                languageOutput.AppendLine();
            }

            var dir = Path.Combine(path, DirName, country, result.Language);
            File.WriteAllText(Path.Combine(dir, String.Format("report-{0}-{1}.txt", country, result.Language)), languageOutput.ToString());
        }

        private void WriteModelReport(ModelCompareResult result, string country, string language, StringBuilder output, string path)
        {
            var modelOutput = new StringBuilder();

            modelOutput.AppendLine("Result for {0}-{1} {2}:", country, language, result.ModelName);
            modelOutput.AppendLine();

            modelOutput.AppendLine("Valid: {0} | Total mismatches: {1} | Total missing: {2} | Total misorders: {3} | Total exceptions: {4} | Total not implemented: {5}",
                                result.Result.IsValid, result.Result.Mismatches.Count, result.Result.Missing.Count, result.Result.Misorders.Count, result.Result.Exceptions.Count, result.Result.NotImplemented.Count);
            modelOutput.AppendLine("---------------------------------------");

            output.Append(modelOutput.ToString());

            modelOutput.AppendLine();

            if (result.Result.Mismatches.Any())
            {
                modelOutput.AppendLine("Mismatches ({0}):", result.Result.Mismatches.Count);
                foreach (var item in result.Result.Mismatches)
                    modelOutput.AppendLine(item);
                modelOutput.AppendLine();
            }

            if (result.Result.Missing.Any())
            {
                modelOutput.AppendLine("Missing ({0}):", result.Result.Missing.Count);
                foreach (var item in result.Result.Missing)
                    modelOutput.AppendLine(item);
                modelOutput.AppendLine();
            }

            if (result.Result.Misorders.Any())
            {
                modelOutput.AppendLine("Misorders ({0}):", result.Result.Misorders.Count);
                foreach (var item in result.Result.Misorders)
                    modelOutput.AppendLine(item);
                modelOutput.AppendLine();
            }

            if (result.Result.Exceptions.Any())
            {
                modelOutput.AppendLine("Exceptions ({0}):", result.Result.Exceptions.Count);
                foreach (var item in result.Result.Exceptions)
                    modelOutput.AppendLine(item);
                modelOutput.AppendLine();
            }

            if (result.Result.NotImplemented.Any())
            {
                modelOutput.AppendLine("Not implemented ({0}):", result.Result.NotImplemented.Count);
                foreach (var item in result.Result.NotImplemented)
                    modelOutput.AppendLine(item);
                modelOutput.AppendLine();
            }

            var dir = Path.Combine(path, DirName, country, language);
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, String.Format("report-{0}-{1}-{2}.txt", country, language, result.ModelName)), modelOutput.ToString());
        }

    }
}
