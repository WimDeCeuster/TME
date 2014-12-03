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
        
        public AccentColourCombinationBuilder WithPrimaryColour(ExteriorColour primary)
        {
            _accentColourCombination.PrimaryAccentColour = primary;
            return this;
        }
        
        public AccentColourCombinationBuilder WithSecondaryColour(ExteriorColour secondary)
        {
            _accentColourCombination.SecondaryAccentColour = secondary;
            return this;
        }

        public AccentColourCombination Build()
        {
            return _accentColourCombination;
        }
    }
}