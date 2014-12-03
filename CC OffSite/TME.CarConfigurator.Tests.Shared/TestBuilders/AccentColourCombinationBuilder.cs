using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class AccentColourCombinationBuilder
    {
        readonly AccentColourCombination _accentColourCombination;

        public AccentColourCombinationBuilder()
        {
            _accentColourCombination = new AccentColourCombination();
        }

        public AccentColourCombinationBuilder WithBodyColour(ExteriorColour bodyColour)
        {
            _accentColourCombination.BodyColour = bodyColour;
            return this;
        }

        public AccentColourCombination Build()
        {
            return _accentColourCombination;
        }
    }
}