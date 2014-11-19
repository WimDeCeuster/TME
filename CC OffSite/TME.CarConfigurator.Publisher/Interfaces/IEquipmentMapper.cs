using System;
using System.Collections.Generic;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Repository.Objects.Equipment;
using Car = TME.CarConfigurator.Administration.Car;
using CarAccessory = TME.CarConfigurator.Repository.Objects.Equipment.CarAccessory;
using CarOption = TME.CarConfigurator.Repository.Objects.Equipment.CarOption;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IEquipmentMapper
    {
        GradeAccessory MapGradeAccessory(ModelGenerationGradeAccessory generationGradeAccessory, Administration.Accessory crossModelAccessory, EquipmentCategories categories, IReadOnlyList<Car> cars, Boolean isPreview, ExteriorColourTypes exteriorColourTypes);
        GradeOption MapGradeOption(ModelGenerationGradeOption generationGradeOption, Administration.Option crossModelOption, EquipmentCategories categories, IReadOnlyList<Car> cars, Boolean isPreview, ExteriorColourTypes exteriorColourTypes);
        CarAccessory MapCarAccessory(Administration.CarAccessory generationCarAccessory, Administration.Accessory crossModelAccessory, EquipmentCategories categories, bool isPreview, IEnumerable<Car> cars, ExteriorColourTypes exteriorColourTypes);
        CarOption MapCarOption(Administration.CarOption generationCarOption, Administration.Option crossModelOption, EquipmentCategories categories, bool isPreview, IEnumerable<Car> cars, ExteriorColourTypes exteriorColourTypes);
    }
}
