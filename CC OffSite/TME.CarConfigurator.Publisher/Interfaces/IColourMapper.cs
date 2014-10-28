using TME.CarConfigurator.Administration;
using ExteriorColour = TME.CarConfigurator.Repository.Objects.Colours.ExteriorColour;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IColourMapper
    {
        ExteriorColour MapExteriorColour(ModelGenerationExteriorColour colour, string colourFilePath);
        ExteriorColour MapColourCombination(ModelGenerationExteriorColour exteriorColour);
        ExteriorColour MapExteriorColour(Administration.ExteriorColour crossModelColour, string colourFilePath);
    }
}
