using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Publisher.Common.Enums;

namespace TME.CarConfigurator.Publisher.Job
{
    internal class Configuration
    {
        public string Brand { get; set; }
        public IReadOnlyCollection<string> Countries { get; set; }
        public string Environment { get; set; }
        public PublicationDataSubset Datasubset { get; set; }
        public string Target { get; set; }

        public Configuration(string brand, string countries, string environment, string datasubset, string target)
        {
            Brand = brand;
            Countries = countries.Split(',').ToList();
            Environment = environment;
            Target = target;
            Datasubset = (PublicationDataSubset)Enum.Parse(typeof(PublicationDataSubset), datasubset, true);
        }
    }
}