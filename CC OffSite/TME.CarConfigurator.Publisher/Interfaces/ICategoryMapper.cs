using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ICategoryMapper
    {
        CategoryInfo MapEquipmentCategoryInfo(EquipmentCategoryInfo categoryInfo, Administration.EquipmentCategories categories);
        Category MapEquipmentCategory(EquipmentCategory category);
    }
}
