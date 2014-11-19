using System;
using System.Collections.Generic;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Repository.Objects.Equipment;
using Car = TME.CarConfigurator.Administration.Car;
using CarAccessory = TME.CarConfigurator.Repository.Objects.Equipment.CarAccessory;
using CarOption = TME.CarConfigurator.Repository.Objects.Equipment.CarOption;
using EquipmentItem = TME.CarConfigurator.Administration.EquipmentItem;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IEquipmentMapper
    {
        GradeAccessory MapGradeAccessory(ModelGenerationGradeAccessory generationGradeAccessory, Administration.EquipmentItem crossModelAccessory, EquipmentCategories categories, IReadOnlyList<Car> cars, Boolean isPreview, ExteriorColourTypes exteriorColourTypes, String assetUrl);
        GradeOption MapGradeOption(ModelGenerationGradeOption generationGradeOption, Administration.EquipmentItem crossModelOption, EquipmentCategories categories, IReadOnlyList<Car> cars, Boolean isPreview, ExteriorColourTypes exteriorColourTypes, String assetUrl);
        CarAccessory MapCarAccessory(Administration.CarAccessory generationCarAccessory, Administration.Accessory crossModelEquipmentItem, EquipmentCategories categories, bool isPreview, IEnumerable<Car> cars, Car car, ExteriorColourTypes exteriorColourTypes, String assetUrl);
        CarOption MapCarOption(Administration.CarOption generationCarOption, Administration.Option crossModelEquipmentItem, EquipmentCategories categories, bool isPreview, IEnumerable<Car> cars, ExteriorColourTypes exteriorColourTypes, String assetUrl);
    }
}
