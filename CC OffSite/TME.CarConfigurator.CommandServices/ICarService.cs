using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.CommandServices
{
    public interface ICarService
    {
        Task<Result> PutTimeFrameGenerationCars(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<Car> cars);
    }
}
