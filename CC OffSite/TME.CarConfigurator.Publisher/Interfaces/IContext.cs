using System;
using System.Collections.Generic;
using TME.CarConfigurator.Administration;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IContext
    {
        String Brand { get; }
        String Country { get; }
        IDictionary<String, ModelGeneration> ModelGenerations { get; }
        IDictionary<String, IContextData> ContextData { get; }
        IDictionary<String, IReadOnlyList<TimeFrame>> TimeFrames { get; }
        Guid GenerationID { get; }
    }
}