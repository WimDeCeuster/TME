using System;
using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class CarColourCombinationBuilder
    {
        readonly CarColourCombination _colourCombination;

        public CarColourCombinationBuilder()
        {
            _colourCombination = new CarColourCombination();
        }

        public CarColourCombinationBuilder WithId(Guid id)
        {
            _colourCombination.ID = id;

            return this;
        }

        public CarColourCombinationBuilder WithExteriorColour(CarExteriorColour exteriorColour)
        {
            _colourCombination.ExteriorColour = exteriorColour;

            return this;
        }

        public CarColourCombinationBuilder WithUpholstery(CarUpholstery upholstery)
        {
            _colourCombination.Upholstery = upholstery;

            return this;
        }

        public CarColourCombination Build()
        {
            return _colourCombination;
        }
    }
}