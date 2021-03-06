using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.TechnicalSpecifications;

namespace TME.CarConfigurator.QueryServices
{
    public interface ISpecificationsService
    {
        IEnumerable<Category> GetCategories(Guid publicationId, Context context);
        IEnumerable<CarTechnicalSpecification> GetCarTechnicalSpecifications(Guid publicationId, Guid carId, Context context);
    }
}