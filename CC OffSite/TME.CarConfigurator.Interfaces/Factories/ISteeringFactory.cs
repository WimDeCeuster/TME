using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface ISteeringFactory
    {
        IEnumerable<ISteering> GetSteerings(Publication publication, Context context);
    }
}
