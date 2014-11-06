using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.CommandServices
{
    public interface ICarService
    {
        Task PutTimeFrameGenerationCars(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<Car> cars);
    }
}
