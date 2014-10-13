using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryServices
{
    public interface IBodyTypeService
    {
        IEnumerable<BodyType> GetBodyTypes(Guid publicationId, Guid publicationTimeFrameId, Context context);
    }
}