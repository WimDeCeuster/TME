using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.QueryServices
{
    public interface IColourService
    {
        IEnumerable<ColourCombination> GetColourCombinations(Guid publicationId, Guid publicationTimeFrameId, Context context);
        IEnumerable<CarColourCombination> GetCarColourCombinations(Guid publicationID, Context context, Guid carID);
        IDictionary<Guid, IEnumerable<AccentColourCombination>> GetCarPackAccentColourCombinations(Guid carID, Guid publicationID, Context context);
    }
}
