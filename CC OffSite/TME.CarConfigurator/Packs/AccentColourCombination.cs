using System;
using TME.CarConfigurator.Equipment;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Packs;

namespace TME.CarConfigurator.Packs
{
    public class AccentColourCombination : IAccentColourCombination
    {
        private readonly Repository.Objects.Packs.AccentColourCombination _repoAccentColourCombination;

        public AccentColourCombination(Repository.Objects.Packs.AccentColourCombination repoAccentColourCombination)
        {
            if (repoAccentColourCombination == null) throw new ArgumentNullException("repoAccentColourCombination");

            _repoAccentColourCombination = repoAccentColourCombination;
        }

        public IExteriorColour BodyColour { get { return new ExteriorColour(_repoAccentColourCombination.BodyColour); } }
        public IExteriorColour PrimaryAccentColour { get { return new ExteriorColour(_repoAccentColourCombination.PrimaryAccentColour); } }
        public IExteriorColour SecondaryAccentColour {get { return new ExteriorColour(_repoAccentColourCombination.SecondaryAccentColour); } }
        public bool Default { get { return _repoAccentColourCombination.Default; } }
    }
}