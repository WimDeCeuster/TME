using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IEngineFactory
    {
        IEnumerable<IEngine> GetEngines(Publication publication, Context context);
    }
}
