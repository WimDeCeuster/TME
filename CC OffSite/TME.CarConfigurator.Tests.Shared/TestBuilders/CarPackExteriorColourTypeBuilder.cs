using System.Linq;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class CarPackExteriorColourTypeBuilder
    {
        private CarPackExteriorColourType _type;

        public CarPackExteriorColourTypeBuilder()
        {
            _type = new CarPackExteriorColourType();
        }

        public CarPackExteriorColourTypeBuilder WithColourCombinations(params ColourCombinationInfo[] colourCombinations)
        {
            _type.ColourCombinations = colourCombinations.ToList();
            return this;
        }

        public CarPackExteriorColourType Build()
        {
            return _type;
        }

    }
}
