using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
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
