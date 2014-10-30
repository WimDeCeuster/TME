using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class ExteriorColourTypeBuilder
    {
        readonly ExteriorColourType _colourType;

        public ExteriorColourTypeBuilder()
        {
            _colourType = new ExteriorColourType();
        }

        public ExteriorColourTypeBuilder WithId(Guid id)
        {
            _colourType.ID = id;

            return this;
        }

        public ExteriorColourType Build()
        {
            return _colourType;
        }
    }
}
