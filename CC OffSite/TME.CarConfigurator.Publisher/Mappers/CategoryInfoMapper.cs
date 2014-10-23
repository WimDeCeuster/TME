using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class CategoryInfoMapper : ICategoryInfoMapper
    {
        public CategoryInfo MapEquipmentCategoryInfo(Administration.EquipmentCategoryInfo categoryInfo)
        {
            var category = Administration.EquipmentCategories.GetEquipmentCategories()[categoryInfo.ID];

            return new CategoryInfo
            {
                ID = category.ID,
                Path = category.Path,
                SortIndex = category.Index
            };
        }
    }
}
