using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;
using ModelGeneration = TME.CarConfigurator.Administration.ModelGeneration;

namespace TME.CarConfigurator.Publisher
{
    public interface IContext
    {
        String Brand { get; }
        String Country { get; }
        PublicationDataSubset DataSubset { get; }
        IDictionary<String, ModelGeneration> ModelGenerations { get; }
        IDictionary<String, IContextData> ContextData { get; }
        IDictionary<String, IReadOnlyList<TimeFrame>> TimeFrames { get; }
        Guid GenerationID { get; }
    }

    public class Context : IContext
    {
        public IDictionary<String, ModelGeneration> ModelGenerations { get; private set; }
        public IDictionary<String, IContextData> ContextData { get; private set; }
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
            ContextData = new Dictionary<String, IContextData>();
            TimeFrames = new Dictionary<String, IReadOnlyList<TimeFrame>>();
        }

    }
}
