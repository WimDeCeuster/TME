using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.TechnicalSpecifications;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class SpecificationCategoryBuilder
    {
        private Category _category;

        public SpecificationCategoryBuilder()
        {
            _category = new Category();
        }

        public SpecificationCategoryBuilder WithId(Guid id)
        {
            _category.ID = id;

            return this;
        }

        public SpecificationCategoryBuilder WithParentId(Guid id)
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
