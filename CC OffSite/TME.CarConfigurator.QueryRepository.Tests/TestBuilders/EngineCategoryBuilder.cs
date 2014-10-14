using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class EngineCategoryBuilder
    {
        private Repository.Objects.EngineCategory _engineCategory;
        private Repository.Objects.Context _context;

        public EngineCategoryBuilder()
        {
            _context = new ContextBuilder().Build();
        }

        public EngineCategoryBuilder WithContext(Repository.Objects.Context context)
        {
            _context = context;

            return this;
        }

        public EngineCategoryBuilder WithEngineCategory(Repository.Objects.EngineCategory engineCategory)
        {
            _engineCategory = engineCategory;

            return this;
        }

        public IEngineCategory Build()
        {
            return new EngineCategory(_engineCategory, _context);
        }
    }
}
