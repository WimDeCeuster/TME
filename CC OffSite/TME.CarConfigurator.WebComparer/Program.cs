using Awesomium.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.WebComparer
{
    class Program
    {
        private static IList<Comparer> _comparers;
        private static IList<String> _countryLanguages;
        private static string _startUrlTemplate;

        static void Main(string[] args)
        {
            _countryLanguages = args.ToList();

            _startUrlTemplate = ConfigurationManager.AppSettings["StartUrlTemplate"];   
            _comparers = new List<Comparer>();

            WebCore.Initialized += WebCore_Initialized;

            var config = new WebConfig();

            config.UserScript = @"
function getTitle(el) {
    return $(el).children('.title').text().trim();
}

function getFullTitle(el) {
    var title = getTitle(el);
    var id = getDataId(el);
    return title + '(' + id + ')';
}

function getBreadCrumb(el) {
    var item = $(el);
    var parents = item.parents('.object');
    var parentPath = parents.toArray().map(getFullTitle).join(' > ');

    var objectDescription = getFullTitle(item);

    return parentPath ? parentPath + ' > ' + objectDescription : objectDescription;
}

function getMismatchInformation(el) {
    var property = $(el);
    var dataId = getDataId(property);
    var parent = propert.parents('.object').eq(0);
    var linkedObject = parent.data('linked-item');
    var linkedProperty = linkedItem.find('.property[data-id=""' + dataId + '""]').filter(isClosestTo(linkedObject));

    var breadCrumb = getBreadCrumb(parent) + ' > ' + dataId;

    var expected = linkedProperty.text().split(':')[1].trim();
    var actual = property.text().split(':')[1].trim();

    return breadCrumb + ' | Expected: ' + expected + ' | Actual: ' + actual; 
}
            ";

            WebCore.Initialize(config, true);

            WebCore.Run();
        }

        static void WebCore_Initialized(object sender, CoreStartEventArgs e)
        {
            
            foreach (var countryLanguage in _countryLanguages)
            {
                var parts = countryLanguage.Split('|');
                var country = parts[0];
                var language = parts[1];

                var comparer = new Comparer(country, language, _startUrlTemplate.Replace("{country}", country).Replace("{language}", language));

                comparer.Finished += comparer_Finished;

                _comparers.Add(comparer);
            }

            _comparers.First().Start();

        }

        static void comparer_Finished(object sender, EventArgs e)
        {
            var next = _comparers.FirstOrDefault(comparer => !comparer.IsFinished);
            if (next != null)
            {
                next.Start();
                return;
            }

            var output = new StringBuilder();

            var totalFailedPages = _comparers.Sum(comparer => comparer.Result.FailedPages.Count);
            var processedPages = _comparers.SelectMany(comparer => comparer.Result.ProcessedPages).ToList();
            var totalProcessedPages = processedPages.Count;
            var invalidPages = processedPages.Where(pageResult => !pageResult.IsValid).ToList();
            var totalInvalidPages = invalidPages.Count;
            var totalValidPages = totalProcessedPages - totalInvalidPages;
            var totalMismatchedProperties = invalidPages.Sum(pageResult => pageResult.MismatchedProperties.Count);
            var totalMissingObjects = invalidPages.Sum(pageResult => pageResult.MissingObjects.Count);
            var totalWrongOrderedObjects = invalidPages.Sum(pageResult => pageResult.WrongOrderedObjects.Count);

            var totalDuration = _comparers.Select(comparer => comparer.Watch.Elapsed).Aggregate((ts1, ts2) => ts1 + ts2);

            output.AppendLine(String.Format("Comparison completed. Duration: {0}", totalDuration));
            output.AppendLine(String.Format("Pages visited: {0} | Pages compared: {1} | Failures: {2}",
                                            totalFailedPages + totalProcessedPages,
                                            totalProcessedPages,
                                            totalFailedPages));
            output.AppendLine(String.Format("Valid pages: {0} | Invalid pages: {1}",
                                            totalValidPages, totalInvalidPages));
            output.AppendLine(String.Format("Mismatched properties: {0} | Missing objects: {1} | Wrong ordered objects: {2}",
                                            totalMismatchedProperties, totalMissingObjects, totalWrongOrderedObjects));
            output.AppendLine("-------------------------");
            
            foreach (var comparer in _comparers)
            {
                output.AppendLine();
                ComparerResultReport(comparer, output);
            }

            System.IO.File.WriteAllText("output.txt", output.ToString());

            WebCore.Shutdown();
        }

        static void ComparerResultReport(Comparer comparer, StringBuilder output)
        {
            var compareResult = comparer.Result;

            var totalMismatchedProperties = compareResult.ProcessedPages.SelectMany(pageResult => pageResult.MismatchedProperties).Count();
            var totalMissingObjects = compareResult.ProcessedPages.SelectMany(pageResult => pageResult.MissingObjects).Count();
            var totalWrongOrderedObjects = compareResult.ProcessedPages.SelectMany(pageResult => pageResult.WrongOrderedObjects).Count();
            var totalValidPages = compareResult.ProcessedPages.Count(pageResult => pageResult.IsValid);

            output.AppendLine(String.Format("Comparison for {0}-{1}. Duration: {2}", comparer.Country, comparer.Language, comparer.Watch.Elapsed));
            output.AppendLine(String.Format("Pages visited: {0} | Pages compared: {1} | Failures: {2}",
                                            compareResult.FailedPages.Count + compareResult.ProcessedPages.Count,
                                            compareResult.ProcessedPages.Count,
                                            compareResult.FailedPages.Count));
            output.AppendLine(String.Format("Valid pages: {0} | Invalid pages: {1}",
                                            totalValidPages, compareResult.ProcessedPages.Count - totalValidPages));
            output.AppendLine(String.Format("Mismatched properties: {0} | Missing objects: {1} | Wrong ordered objects: {2}",
                                            totalMismatchedProperties, totalMissingObjects, totalWrongOrderedObjects));
            output.AppendLine("-------------------------");
            output.AppendLine();

            if (compareResult.FailedPages.Any())
            {
                output.AppendLine("Failures:");
                output.AppendLine("-------------------------");
                foreach (var failure in compareResult.FailedPages)
                    output.AppendLine(String.Format("{0}: {1}", failure.Url, failure.Reason));

                output.AppendLine();
            }

            if (totalMismatchedProperties > 0)
            {
                output.AppendLine("Mismatched properties:");
                output.AppendLine("-------------------------");
                output.AppendLine();
                foreach (var pageResult in compareResult.ProcessedPages)
                    foreach (var mismatchedProperty in pageResult.MismatchedProperties)
                        output.AppendLine(String.Format("{0}: {1}", pageResult.Url, mismatchedProperty));
                output.AppendLine();
            }

            if (totalMissingObjects > 0)
            {
                output.AppendLine("Missing objects:");
                output.AppendLine("-------------------------");
                output.AppendLine();
                foreach (var pageResult in compareResult.ProcessedPages)
                    foreach (var missingObject in pageResult.MissingObjects)
                        output.AppendLine(String.Format("{0}: {1}", pageResult.Url, missingObject));
                output.AppendLine();
            }

            if (totalWrongOrderedObjects > 0)
            {
                output.AppendLine("Wrong ordered objects:");
                output.AppendLine("-------------------------");
                output.AppendLine();
                foreach (var pageResult in compareResult.ProcessedPages)
                    foreach (var wrongOrderedObject in pageResult.WrongOrderedObjects)
                        output.AppendLine(String.Format("{0}: {1}", pageResult.Url, wrongOrderedObject));
                output.AppendLine();
            }

            output.AppendLine("All pages:");
            output.AppendLine("-------------------------");
            output.AppendLine();
            foreach (var pageResult in compareResult.ProcessedPages.OrderBy(result => result.Url))
                output.AppendLine(String.Format("{0}: {1}", pageResult.Url, pageResult.IsValid ? "Valid" : "Invalid"));
        }
    }
}