using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.QueryServices
{
    public interface IEquipmentService
    {
        GradeEquipment GetGradeEquipment(Guid publicationId, Guid timeFrameId, Guid gradeId, Context context);
        GradeEquipment GetSubModelGradeEquipment(Guid publicationID, Guid timeFrameID, Guid gradeID, Guid subModelID, Context context);
        IEnumerable<Category> GetCategories(Guid publicationId, Guid timeFrameId, Context context);
        CarEquipment GetCarEquipment(Guid carID, Guid publicationID , Context context);
    }
}