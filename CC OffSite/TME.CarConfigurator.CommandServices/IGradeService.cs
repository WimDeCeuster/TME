using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.CommandServices
{
    public interface IGradeService
    {
        Task PutTimeFrameGenerationGrades(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<Grade> grades);
        Task PutGradesPerSubModel(String brand, String country, Guid publicationID, Guid timeFrameID, Guid subModelID, List<Grade> gradesPerSubModel);
    }
}
