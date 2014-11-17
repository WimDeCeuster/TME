using System;
using System.Collections.Generic;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Repository.Objects.Equipment;
using Car = TME.CarConfigurator.Administration.Car;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IEquipmentMapper
    {
        GradeAccessory MapGradeAccessory(ModelGenerationGradeAccessory generationGradeAccessory, Administration.EquipmentItem crossModelAccessory, EquipmentCategories categories, IReadOnlyList<Car> cars, Boolean isPreview);
        GradeOption MapGradeOption(ModelGenerationGradeOption generationGradeOption, Administration.EquipmentItem crossModelOption, EquipmentCategories categories, IReadOnlyList<Car> cars, Boolean isPreview);
    }
}
