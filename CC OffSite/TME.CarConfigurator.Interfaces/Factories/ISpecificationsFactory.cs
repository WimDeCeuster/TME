using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.TechnicalSpecifications;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface ISpecificationsFactory
    {
        IModelTechnicalSpecifications GetModelSpecifications(Publication publication, Context context);
        IReadOnlyList<ICategory> GetCategories(Publication publication, Context context);
    }
}
