using System;
using System.Collections.Generic;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IEquipmentMapper
    {
        GradeAccessory MapGradeAccessory(Administration.ModelGenerationGradeAccessory generationGradeAccessory, Administration.ModelGenerationAccessory generationAccessory, Administration.Accessory crossModelAccessory, Administration.EquipmentCategories categories, IReadOnlyList<Administration.Car> cars, Boolean isPreview);
        GradeOption MapGradeOption(Administration.ModelGenerationGradeOption generationGradeOption, Administration.ModelGenerationOption generationOption, Administration.Option crossModelOption, Administration.EquipmentCategories categories, IReadOnlyList<Administration.Car> cars, Boolean isPreview);
        IDictionary<Guid, GradeEquipment> MapSubModelGradeEquipment(ModelGenerationSubModel modelGenerationSubModel, ContextData contextData, bool isPreview);
    }
}
