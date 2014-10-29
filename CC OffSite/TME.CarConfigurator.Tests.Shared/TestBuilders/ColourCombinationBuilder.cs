using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class ColourCombinationBuilder
    {
        readonly ColourCombination _colourCombination;

        public ColourCombinationBuilder()
        {
            _colourCombination = new ColourCombination();
        }

        public ColourCombinationBuilder WithId(Guid id)
        {
            _colourCombination.ID = id;

            return this;
        }

        public ColourCombinationBuilder WithExteriorColour(ExteriorColour exteriorColour)
        {
            _colourCombination.ExteriorColour = exteriorColour;

            return this;
        }

        public ColourCombinationBuilder WithUpholstery(Upholstery upholstery)
        {
            _colourCombination.Upholstery = upholstery;

            return this;
        }

        public ColourCombination Build()
        {
            return _colourCombination;
        }
    }
}
