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

                var result = new AutoComparer().Compare(countries, options.Brand, options.DataSubset);

                var reporter = new Reporter();
                reporter.WriteReport(result);
            }
        }
    }
}
