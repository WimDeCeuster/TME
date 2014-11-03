using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class EquipmentCategoryBuilder
    {
        private Category _category;

        public EquipmentCategoryBuilder()
        {
            _category = new Category();
        }

        public EquipmentCategoryBuilder WithId(Guid id)
        {
            _category.ID = id;

            return this;
        }

        public EquipmentCategoryBuilder WithParentId(Guid id)
        {
            _category.ParentCategoryID = id;

            return this;
        }

        public Category Build()
        {
            return _category;
        }
    }
}
