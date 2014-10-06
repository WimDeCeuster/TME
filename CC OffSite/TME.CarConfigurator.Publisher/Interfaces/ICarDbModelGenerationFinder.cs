using System;
using System.Collections.Generic;
using TME.CarConfigurator.Administration;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ICarDbModelGenerationFinder
    {
        IReadOnlyDictionary<string, Tuple<ModelGeneration, Model>> GetModelGeneration(String brand, String countryCode, Guid generationID);
    }
}