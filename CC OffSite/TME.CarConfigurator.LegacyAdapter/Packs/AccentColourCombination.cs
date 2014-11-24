using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Packs;
using ExteriorColour = TME.CarConfigurator.LegacyAdapter.Equipment.ExteriorColour;

namespace TME.CarConfigurator.LegacyAdapter.Packs
{
    public class AccentColourCombination : IAccentColourCombination
    {
        public AccentColourCombination(TMME.CarConfigurator.AccentColourCombination adaptee)
        {
            BodyColour = new ExteriorColour(adaptee.BodyColour);
            PrimaryAccentColour = new ExteriorColour(adaptee.PrimaryAccentColour);
            SecondaryAccentColour = new ExteriorColour(adaptee.SecondaryAccentColour);
            Default = adaptee.Default;
        }

        public IExteriorColour BodyColour { get; private set; }
        public IExteriorColour PrimaryAccentColour { get; private set; }
        public IExteriorColour SecondaryAccentColour { get; private set; }
        public bool Default { get; private set; }
    }
}
