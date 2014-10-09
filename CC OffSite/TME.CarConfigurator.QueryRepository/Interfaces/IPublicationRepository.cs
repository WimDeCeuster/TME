using System;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository.Interfaces
{
    public interface IPublicationRepository
    {
        Publication GetPublication(Guid publicationID, IContext context);
    }
}
