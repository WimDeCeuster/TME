using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface ISteeringFactory
    {
        IReadOnlyList<ISteering> GetSteerings(Publication publication, Context context);
        ISteering GetCarSteering(Steering steering, Guid carID, Publication publication, Context context);
    }
}
