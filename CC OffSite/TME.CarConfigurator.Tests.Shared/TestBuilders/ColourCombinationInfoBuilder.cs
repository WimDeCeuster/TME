using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class ColourCombinationInfoBuilder
    {
        private ColourCombinationInfo _info;

        public ColourCombinationInfoBuilder()
        {
            _info = new ColourCombinationInfo();
        }

        public ColourCombinationInfoBuilder WithExteriorColour(ExteriorColourInfo exteriorColour)
        {
            _info.ExteriorColour = exteriorColour;
            return this;
        }

        public ColourCombinationInfoBuilder WithUpholstery(UpholsteryInfo upholstery)
        {
            _info.Upholstery = upholstery;
            return this;
        }

        public ColourCombinationInfo Build()
        {
            return _info;
        }
    }
}
