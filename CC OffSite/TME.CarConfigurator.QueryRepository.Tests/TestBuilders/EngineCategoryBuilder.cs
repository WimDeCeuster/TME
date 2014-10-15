using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class EngineCategoryBuilder
    {
        private Repository.Objects.EngineCategory _engineCategory;
        
        public EngineCategoryBuilder WithEngineCategory(Repository.Objects.EngineCategory engineCategory)
        {
            _engineCategory = engineCategory;

            return this;
        }

        public IEngineCategory Build()
        {
            return new EngineCategory(_engineCategory);
        }
    }
}
