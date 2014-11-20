using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.CommandServices
{
    public interface ICarPartService
    {
        Task PutCarParts(String brand, String country, Guid publicationID, Guid carID, IEnumerable<CarPart> carParts);
    }
}