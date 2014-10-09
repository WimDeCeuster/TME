using System;
using System.Collections.Generic;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Common.Enums;

namespace TME.CarConfigurator.Publisher.Common.Interfaces
{
    public interface IContext
    {
        PublicationDataSubset DataSubset { get; }
        String Brand { get; }
        String Country { get; }
        IDictionary<String, ModelGeneration> ModelGenerations { get; }
        IDictionary<String, ContextData> ContextData { get; }
        IDictionary<String, IReadOnlyList<TimeFrame>> TimeFrames { get; }
        Guid GenerationID { get; }
    }
}