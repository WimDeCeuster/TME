using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.CommandRepository
{
    public interface ICarRepository
    {
        Result.Result Create(Repository.Objects.Context.PublicationTimeFrame context, IEnumerable<Car> cars);
    }
}
