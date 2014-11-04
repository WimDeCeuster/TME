using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.CommandServices
{
    public interface IColourService
    {
        Task PutTimeFrameGenerationColourCombinations(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<ColourCombination> colourCombinations);
    }
}