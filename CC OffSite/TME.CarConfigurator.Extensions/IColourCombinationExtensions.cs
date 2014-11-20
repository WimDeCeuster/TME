using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator.Extensions
{
    public static class ColourCombinationExtensions
    {
        public static IEnumerable<IExteriorColour> ExteriorColours(this IEnumerable<IColourCombination> colourCombinations)
        {
            return colourCombinations.Select(x => x.ExteriorColour).Distinct();
        }
        public static IEnumerable<IUpholstery> Upholsteries(this IEnumerable<IColourCombination> colourCombinations)
        {
            return colourCombinations.Select(x => x.Upholstery).Distinct();
        }
    }
}
