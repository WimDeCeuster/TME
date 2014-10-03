using System;
using System.Collections.Generic;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Enums;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IContext
    {
        PublicationDataSubset DataSubset { get; }
        String Brand { get; }
        String Country { get; }
        IDictionary<String, ModelGeneration> ModelGenerations { get; }
        IDictionary<String, IContextData> ContextData { get; }
        IDictionary<String, IReadOnlyList<TimeFrame>> TimeFrames { get; }
        Guid GenerationID { get; }
    }
}