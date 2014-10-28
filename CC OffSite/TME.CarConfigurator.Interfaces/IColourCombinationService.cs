using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.Interfaces
{
    public interface IColourCombinationService
    {
        Task<Result> PutTimeFrameGenerationColourCombinationsAsync(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<ExteriorColour> colourCombinations);
    }
}