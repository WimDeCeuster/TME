using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.CommandServices
{
    public interface IGradePackService
    {
        Task PutAsync(string brand, string country, Guid publicationId, Guid timeFrameId, Guid gradeId, IList<GradePack> packs);
    }
}