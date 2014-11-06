using EquipmentCategoryInfo = TME.CarConfigurator.Repository.Objects.Equipment.CategoryInfo;
using EquipmentCategory = TME.CarConfigurator.Repository.Objects.Equipment.Category;
using SpecificationCategory = TME.CarConfigurator.Repository.Objects.TechnicalSpecifications.Category;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ICategoryMapper
    {
        EquipmentCategoryInfo MapEquipmentCategoryInfo(Administration.EquipmentCategoryInfo categoryInfo, Administration.EquipmentCategories categories);
        EquipmentCategory MapEquipmentCategory(Administration.EquipmentCategory category);
        SpecificationCategory MapSpecificationCategory(Administration.SpecificationCategory category);
    }
}
