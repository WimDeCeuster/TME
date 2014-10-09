using System;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryServices
{
    public interface IPublicationService
    {
        Publication GetPublication(Guid publicationId, Context context);
    }
}