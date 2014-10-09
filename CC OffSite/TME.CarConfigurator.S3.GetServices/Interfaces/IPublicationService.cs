using System;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.S3.GetServices.Interfaces
{
    public interface IPublicationService
    {
        Publication GetPublication(Guid publicationId, Context context);
    }
}