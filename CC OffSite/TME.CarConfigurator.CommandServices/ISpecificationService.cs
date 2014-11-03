using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.TechnicalSpecifications;

namespace TME.CarConfigurator.CommandServices
{
    public interface ISpecificationsService
    {
        Task<Result> PutCategoriesAsync(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<Category> categories);
    }
}
