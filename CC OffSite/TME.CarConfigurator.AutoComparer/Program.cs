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

            var start = DateTime.Now;


            if (options.ReadKeyAfterFinish)
                Console.WriteLine("Started at {0}\n", start);

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

            if (!options.ReadKeyAfterFinish) return;

            Console.WriteLine("\nDone after {0} seconds", DateTime.Now.Subtract(start).TotalMilliseconds / 1000);
            Console.ReadKey();
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
