using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class ExteriorColourInfoBuilder
    {
        private ExteriorColourInfo _exteriorColourInfo;

        public ExteriorColourInfoBuilder()
        {
            _exteriorColourInfo = new ExteriorColourInfo();
        }

        public ExteriorColourInfoBuilder WithId(Guid id)
        {
            _exteriorColourInfo.ID = id;
            return this;
        }

        public ExteriorColourInfo Build()
        {
            return _exteriorColourInfo;
        }
    }
}
