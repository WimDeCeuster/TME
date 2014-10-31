using System;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.QueryServices
{
    public interface IGradeEquipmentService
    {
        GradeEquipment GetGradeEquipment(Guid publicationId, Guid timeFrameId, Guid gradeId, Context context);
        GradeEquipment GetSubModelGradeEquipment(Guid publicationID, Guid timeFrameID, Guid gradeID, Guid subModelID, Context context);
    }
}