using System;
using System.Collections.Generic;
using TME.CarConfigurator.Publisher.Interfaces;
using ModelGeneration = TME.CarConfigurator.Administration.ModelGeneration;
using TME.CarConfigurator.Publisher.Enums;

namespace TME.CarConfigurator.Publisher
{
    public class Context : IContext
    {
        public IDictionary<String, ModelGeneration> ModelGenerations { get; private set; }
        public IDictionary<String, ContextData> ContextData { get; private set; }
        public IDictionary<String, IReadOnlyList<TimeFrame>> TimeFrames { get; private set; }
        public Guid GenerationID { get; private set; }

        public PublicationDataSubset DataSubset { get; private set; }

        public String Brand { get; private set; }
        public String Country { get; private set; }

        public Context(String brand, String country, Guid generationID, PublicationDataSubset dataSubset)
        {
            DataSubset = dataSubset;
            Brand = brand;
            Country = country;

            GenerationID = generationID;

            ModelGenerations = new Dictionary<String, ModelGeneration>();
            ContextData = new Dictionary<String, ContextData>();
            TimeFrames = new Dictionary<String, IReadOnlyList<TimeFrame>>();
        }

    }
}
