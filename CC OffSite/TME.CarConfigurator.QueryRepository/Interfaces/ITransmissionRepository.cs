using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository.Interfaces
{
    public interface ITransmissionRepository
    {
        IEnumerable<Transmission> GetTransmissions(Repository.Objects.Context.PublicationTimeFrame context);
    }
}
