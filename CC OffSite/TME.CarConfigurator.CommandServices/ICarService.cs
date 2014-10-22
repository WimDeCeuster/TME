using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.CommandServices
{
    public interface ICarService
    {
        Task<Result> PutTimeFrameGenerationCars(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<Car> cars);
    }
}
