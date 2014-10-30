using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.CommandServices
{
    public interface IGradeEquipmentService
    {
        Task<Result> Put(String brand, String country, Guid publicationID, Guid timeFrameID, Guid gradeID, GradeEquipment gradeEquipment);
        Task<Result> PutPerSubModel(String brand, String country, Guid publicationID, Guid timeFrameID, Guid subModelID, Guid gradeID, GradeEquipment sortGradeEquipment);
    }
}
