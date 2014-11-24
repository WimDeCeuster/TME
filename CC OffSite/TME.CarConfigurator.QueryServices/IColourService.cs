using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.QueryServices
{
    public interface IColourService
    {
        IEnumerable<ColourCombination> GetColourCombinations(Guid publicationId, Guid publicationTimeFrameId, Context context);
        IEnumerable<ColourCombination> GetCarColourCombinations(Guid publicationID, Context context, Guid carID);
    }
}
