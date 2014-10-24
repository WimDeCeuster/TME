using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IEquipmentMapper
    {
        GradeAccessory MapGradeAccessory(Administration.ModelGenerationGradeAccessory generationGradeAccessory, Administration.ModelGenerationAccessory generationAccessory, Administration.Accessory crossModelAccessory, Administration.EquipmentCategories categories, Boolean isPreview);
        GradeOption MapGradeOption(Administration.ModelGenerationGradeOption generationGradeOption, Administration.ModelGenerationOption generationOption, Administration.Option crossModelOption, Administration.EquipmentCategories categories, Boolean isPreview);
    }
}
