﻿using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IEquipmentFactory
    {
        IGradeEquipment GetGradeEquipment(Publication publication, Context context, Guid gradeId);
        IModelEquipment GetModelEquipment(Publication publication, Context context);
        IReadOnlyList<ICategory> GetCategories(Publication publication, Context context);
    }
}
