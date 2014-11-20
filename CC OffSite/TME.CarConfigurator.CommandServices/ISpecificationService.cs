using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.TechnicalSpecifications;

namespace TME.CarConfigurator.CommandServices
{
    public interface ISpecificationsService
    {
        Task PutCategoriesAsync(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<Category> categories);
        Task PutCarTechnicalSpecificationsAsync(String brand, String country, Guid publicationID,  Guid carID, IEnumerable<CarTechnicalSpecification> technicalSpecifications);
    }
}
