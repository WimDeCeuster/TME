using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface ICarPartFactory
    {
        IReadOnlyList<CarPart> GetCarCarParts(Guid carID, Publication publication, Context context);
    }
}