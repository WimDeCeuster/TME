using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.CommandServices
{
    public interface IColourService
    {
        Task PutTimeFrameGenerationColourCombinations(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<ColourCombination> colourCombinations);
        Task PutCarColourCombinations(String brand, String country, Guid publicationID, Guid carID, IList<CarColourCombination> carColourCombinations);
        Task PutCarPackAccentColourCombinations(String brand, String country, Guid publicationID, Guid carID, IDictionary<Guid, List<AccentColourCombination>> carPackAccentColourCombinations);
    }
}