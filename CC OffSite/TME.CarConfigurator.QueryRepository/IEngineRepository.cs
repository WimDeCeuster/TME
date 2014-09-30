using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository
{
    public interface IEngineRepository
    {
        IEnumerable<Engine> GetEngines(Repository.Objects.Context.PublicationTimeFrame context);
    }
}
