using System;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IGradeEquipmentFactory
    {
        IGradeEquipment GetGradeEquipment(Publication publication, Context context, Guid gradeId);
        IGradeEquipment GetSubModelGradeEquipment(Publication publication, Guid subModelID, Context context, Guid gradeID);
    }
}
