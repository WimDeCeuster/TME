using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.S3.Publisher.Extensions
{
    public static class CarPackAccentColourCombinationExtensions
    {
        public static List<AccentColourCombination> Sort(this IEnumerable<AccentColourCombination> colourCombinations)
        {
            return colourCombinations.OrderBy(colourCombination => colourCombination.BodyColour.InternalCode)
                .ThenBy(colourCombination => colourCombination.PrimaryAccentColour.InternalCode)
                .ThenBy(colourCombination => colourCombination.SecondaryAccentColour.InternalCode)
                .ToList();
        }
    }
}