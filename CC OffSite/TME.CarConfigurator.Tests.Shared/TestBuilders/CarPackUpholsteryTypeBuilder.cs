using System.Linq;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class CarPackUpholsteryTypeBuilder
    {
        private CarPackUpholsteryType _type;

        public CarPackUpholsteryTypeBuilder()
        {
            _type = new CarPackUpholsteryType();
        }

        public CarPackUpholsteryTypeBuilder WithColourCombinations(params ColourCombinationInfo[] colourCombinations)
        {
            _type.ColourCombinations = colourCombinations.ToList();
            return this;
        }

        public CarPackUpholsteryType Build()
        {
            return _type;
        }

    }
}
