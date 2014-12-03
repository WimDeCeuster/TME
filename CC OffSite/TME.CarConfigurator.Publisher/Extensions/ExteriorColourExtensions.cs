using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;

namespace TME.CarConfigurator.Publisher.Extensions
{
    public static class ExteriorColourExtensions
    {
        public static IEnumerable<ModelGenerationExteriorColour> Filter(this IEnumerable<ModelGenerationExteriorColour> colourCombinations, Car car)
        {
            return car.ColourCombinations.ExteriorColours()
                .SelectMany(cec => colourCombinations
                    .Where(cc => cc.ID == cec.ID));
        }
    }
}