using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IGradeEquipmentFactory
    {
        IGradeEquipment GetGradeEquipment(Publication publication, Context context, Guid gradeId);
    }
}
