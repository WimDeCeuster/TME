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
        public string PublishedBy { get; set; }

        public Configuration(string brand, string countries, string environment, string datasubset, string target, string publishedBy)
        {
            Brand = brand;
            Countries = countries.Split(',').ToList();
            Environment = environment;
            Target = target;
            PublishedBy = publishedBy;
            Datasubset = (PublicationDataSubset)Enum.Parse(typeof(PublicationDataSubset), datasubset, true);
        }

    }
}