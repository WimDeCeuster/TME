using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.CommandServices
{
    public interface IEquipmentService
    {
        Task Put(String brand, String country, Guid publicationID, Guid timeFrameID, Guid gradeID, GradeEquipment gradeEquipment);
        Task PutCategoriesAsync(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<Category> categories);
        Task PutPerSubModel(String brand, String country, Guid publicationID, Guid timeFrameID, Guid subModelID, Guid gradeID, GradeEquipment sortGradeEquipment);
    }
}
