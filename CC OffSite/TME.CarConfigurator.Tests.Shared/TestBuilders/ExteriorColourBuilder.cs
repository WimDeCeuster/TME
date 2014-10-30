using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class ExteriorColourBuilder
    {
        readonly ExteriorColour _colour;

        public ExteriorColourBuilder()
        {
            _colour = new ExteriorColour();
        }

        public ExteriorColourBuilder WithId(Guid id)
        {
            _colour.ID = id;

            return this;
        }

        public ExteriorColourBuilder WithExteriorColourType(ExteriorColourType type)
        {
            _colour.Type = type;

            return this;
        }

        public ExteriorColour Build()
        {
            return _colour;
        }
    }
}
