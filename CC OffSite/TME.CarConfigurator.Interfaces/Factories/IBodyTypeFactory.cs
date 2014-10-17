using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IBodyTypeFactory
    {
        IEnumerable<IBodyType> GetBodyTypes(Publication publication, Context context);

        IBodyType GetCarBodyType(BodyType bodyType, Guid carID, Publication publication, Context context);
    }
}