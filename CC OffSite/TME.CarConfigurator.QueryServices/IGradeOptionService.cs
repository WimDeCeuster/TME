using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.QueryServices
{
    public interface IGradeEquipmentService
    {
        GradeEquipment GetGradeEquipment(Guid publicationId, Guid timeFrameId, Guid gradeId, Context context);
    }
}