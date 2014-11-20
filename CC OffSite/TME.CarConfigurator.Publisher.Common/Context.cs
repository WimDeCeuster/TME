using System;
using System.Collections.Generic;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;

namespace TME.CarConfigurator.Publisher.Common
{
    public class Context : IContext
    {
        public IDictionary<String, ModelGeneration> ModelGenerations { get; private set; }
        public IDictionary<String, ContextData> ContextData { get; private set; }
        public IDictionary<String, IReadOnlyList<TimeFrame>> TimeFrames { get; private set; }
        public Guid GenerationID { get; private set; }

        public PublicationDataSubset DataSubset { get; private set; }
        public String AssetUrl { get; private set; }
        public string PublishedBy { get; private set; }

        public String Brand { get; private set; }
        public String Country { get; private set; }

        public Context(string brand, string country, Guid generationID, PublicationDataSubset dataSubset, string assetUrl, string publishedBy)
        {
            DataSubset = dataSubset;
            Brand = brand;
            Country = country;
            AssetUrl = assetUrl;
            PublishedBy = publishedBy;

            GenerationID = generationID;

            ModelGenerations = new Dictionary<String, ModelGeneration>();
            ContextData = new Dictionary<String, ContextData>();
            TimeFrames = new Dictionary<String, IReadOnlyList<TimeFrame>>();
        }

    }
}
