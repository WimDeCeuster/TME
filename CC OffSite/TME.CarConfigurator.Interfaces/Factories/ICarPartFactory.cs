using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface ICarPartFactory
    {
        IReadOnlyList<ICarPart> GetCarCarParts(Guid carID, Publication publication, Context context);
    }
}