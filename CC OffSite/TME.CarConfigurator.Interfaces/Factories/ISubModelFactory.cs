using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface ISubModelFactory
    {
        IReadOnlyList<ISubModel> GetSubModels(Publication publication, Context context);
        ISubModel GetCarSubModel(SubModel subModel, Guid carID, Publication publication, Context context);
    }
}