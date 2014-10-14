using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class EngineCategoryBuilder
    {
        private EngineCategory _category;

        public EngineCategoryBuilder()
        {
            _category = new EngineCategory();
        }

        public EngineCategoryBuilder WithId(Guid id)
        {
            _category.ID = id;

            return this;
        }

        public EngineCategoryBuilder WithLabels(params Repository.Objects.Core.Label[] labels)
        {
            _category.Labels = labels.ToList();

            return this;
        }

        public EngineCategory Build()
        {
            return _category;
        }
    }
}
