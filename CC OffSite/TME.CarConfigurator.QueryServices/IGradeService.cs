
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryServices
{
    public interface IGradeService
    {
        IEnumerable<Grade> GetGrades(Guid publicationId, Guid publicationTimeFrameId, Context context);
    }
}
