using System;
using System.Collections.Generic;

namespace TME.CarConfigurator.QueryRepository
{
    public interface IPublicationRepository
    {
        IEnumerable<Repository.Objects.Publication> GetPublication(Repository.Objects.Context.Base context, Guid publicationID);
    }
}
