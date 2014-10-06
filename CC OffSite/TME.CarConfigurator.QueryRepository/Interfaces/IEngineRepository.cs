using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository.Interfaces
{
    public interface IEngineRepository
    {
        IEnumerable<Engine> GetEngines(Repository.Objects.Context.PublicationTimeFrame context);
    }
}
