using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryServices
{
    public interface ICarPartService
    {
        IEnumerable<CarPart> GetCarParts(Guid publicationID, Guid carID, Context context);
    }
}