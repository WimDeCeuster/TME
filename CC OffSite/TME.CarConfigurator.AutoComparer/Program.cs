using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace TME.CarConfigurator.AutoComparer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();

            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                Console.WriteLine("Could not parse args");
                return;
            }

            try
            {
                var countries = GetCountries(options);

                var result = new AutoComparer().Compare(countries, options.Brand, options.DataSubset);

                var reporter = new Reporter();
                reporter.WriteReport(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static IList<string> GetCountries(Options options)
        {
            var countries = options.Countries;

            if (countries.Any()) return countries;

            countries = (ConfigurationManager.AppSettings["Countries"] ?? "").Split(' ').ToList();

            if (!countries.Any())
                throw new Exception("No country codes passed to process");

            return countries;
        }
    }
}
