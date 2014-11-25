using System;
using TME.CarConfigurator.Repository.Objects.TechnicalSpecifications;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders.TechnicalSpecifications
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

        public CategoryInfoBuilder WithSortOrder(int sortOrder)
        {
            _category.SortIndex = sortOrder;
            return this;
        }

        public CategoryInfo Build()
        {
            return _category;
        }
    }
}