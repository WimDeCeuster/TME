using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository
{
    public interface ICarRepository
    {
        IEnumerable<Car> GetCars(Repository.Objects.Context.PublicationTimeFrame context);
    }
}
