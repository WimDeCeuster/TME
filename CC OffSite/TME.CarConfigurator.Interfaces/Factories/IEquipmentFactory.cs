using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IEquipmentFactory
    {
        IGradeEquipment GetGradeEquipment(Publication publication, Context context, Guid gradeId);
        IGradeEquipment GetSubModelGradeEquipment(Publication publication, Guid subModelID, Context context, Guid gradeID);
        IModelEquipment GetModelEquipment(Publication publication, Context context);
        IReadOnlyList<ICategory> GetCategories(Publication publication, Context context);
        ICarEquipment GetCarEquipment(Guid carID, Publication publication, Context repositoryContext);
        ICarPackEquipment GetCarPackEquipment(CarPackEquipment carPackEquipment, Publication publication, Context context, Guid carId);
    }
}
