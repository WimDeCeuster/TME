using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.CommandServices
{
    public interface ISteeringService
    {
        Task PutTimeFrameGenerationSteerings(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<Steering> steerings);
    }
}
