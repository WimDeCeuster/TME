using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryServices
{
    public interface ISubModelService
    {
        IEnumerable<SubModel> GetSubModels(Guid publicationId, Guid publicationTimeFrameId, Context context);
    }
}