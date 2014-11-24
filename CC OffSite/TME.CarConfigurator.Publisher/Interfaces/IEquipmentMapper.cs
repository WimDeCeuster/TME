using System;
using System.Collections.Generic;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Repository.Objects.Equipment;
using Car = TME.CarConfigurator.Administration.Car;
using CarAccessory = TME.CarConfigurator.Repository.Objects.Equipment.CarAccessory;
using CarOption = TME.CarConfigurator.Repository.Objects.Equipment.CarOption;
using CarPackAccessory = TME.CarConfigurator.Repository.Objects.Packs.CarPackAccessory;
using CarPackOption = TME.CarConfigurator.Repository.Objects.Packs.CarPackOption;
using CarPackExteriorColourType = TME.CarConfigurator.Repository.Objects.Packs.CarPackExteriorColourType;
using CarPackUpholsteryType = TME.CarConfigurator.Repository.Objects.Packs.CarPackUpholsteryType;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IEquipmentMapper
    {
        GradeAccessory MapGradeAccessory(ModelGenerationGradeAccessory generationGradeAccessory, Administration.Accessory crossModelAccessory, EquipmentCategories categories, IReadOnlyList<Car> cars, Boolean isPreview, ExteriorColourTypes exteriorColourTypes, String assetUrl);
        GradeOption MapGradeOption(ModelGenerationGradeOption generationGradeOption, Administration.Option crossModelOption, EquipmentCategories categories, IReadOnlyList<Car> cars, Boolean isPreview, ExteriorColourTypes exteriorColourTypes, String assetUrl);
        CarAccessory MapCarAccessory(Administration.CarAccessory generationCarAccessory, Administration.Accessory crossModelAccessory, EquipmentCategories categories, bool isPreview, String assetUrl);
        CarOption MapCarOption(Administration.CarOption generationCarOption, Administration.Option crossModelOption, EquipmentCategories categories, bool isPreview, String assetUrl);

        CarPackAccessory MapCarPackAccessory(Administration.CarPackAccessory carPackAccessory, EquipmentGroups groups, EquipmentCategories categories, bool isPreview, string assetUrl);
        CarPackOption MapCarPackOption(Administration.CarPackOption option, EquipmentGroups groups, EquipmentCategories categories, bool isPreview, string assetUrl);
        CarPackExteriorColourType MapCarPackExteriorColourType(Administration.CarPackExteriorColourType type, EquipmentGroups groups, EquipmentCategories categories, bool isPreview, string assetUrl);
        CarPackUpholsteryType MapCarPackUpholsteryType(Administration.CarPackUpholsteryType type, EquipmentGroups groups, EquipmentCategories categories, bool isPreview, string assetUrl);
    }
}
