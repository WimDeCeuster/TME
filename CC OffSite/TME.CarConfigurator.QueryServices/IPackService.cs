using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.QueryServices
{
    public interface IPackService
    {
        IEnumerable<GradePack> GetGradePacks(Guid publicationId, Guid publicationTimeFrameId, Guid gradeId, Context context);
    }
}