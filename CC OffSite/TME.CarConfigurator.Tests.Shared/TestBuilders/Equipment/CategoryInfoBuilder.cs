using System;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders.Equipment
{
    public class CategoryInfoBuilder
    {
        readonly CategoryInfo _category;

        public CategoryInfoBuilder()
        {
            _category = new CategoryInfo();
        }

        public CategoryInfoBuilder WithId(Guid id)
        {
            _category.ID = id;

            return this;
        }

        public CategoryInfo Build()
        {
            return _category;
        }
    }
}
