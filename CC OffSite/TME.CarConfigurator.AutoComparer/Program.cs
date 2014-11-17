using System;
using System.Configuration;
using System.Linq;

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

                var result = new AutoComparer().Compare(countries, options.Brand, options.DataSubset);

                var reporter = new Reporter();
                reporter.WriteReport(result);
            }
        }
    }
}
