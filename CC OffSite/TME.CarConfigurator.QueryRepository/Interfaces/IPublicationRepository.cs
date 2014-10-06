using System;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository.Interfaces
{
    public interface IPublicationRepository
    {
        Publication Get(Guid publicationID);
    }
}
