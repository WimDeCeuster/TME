
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Repository.Objects.Packs
{
    public class AccentColourCombination
    {
        public ExteriorColour BodyColour { get; set; }
        public ExteriorColour PrimaryAccentColour { get; set; }
        public ExteriorColour SecondaryAccentColour { get; set; }
        public bool Default { get; set; }
    }
}
