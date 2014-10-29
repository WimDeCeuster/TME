using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Repository.Objects.Colours;
using ExteriorColour = TME.CarConfigurator.Repository.Objects.Colours.ExteriorColour;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IColourMapper
    {
        ExteriorColour MapExteriorColour(ModelGeneration modelGeneration, ModelGenerationExteriorColour colour, bool isPreview);
        ExteriorColour MapExteriorColour(ModelGeneration modelGeneration, Administration.ExteriorColour crossModelColour, bool isPreview);
        ColourCombination MapColourCombination(ModelGeneration modelGeneration, ModelGenerationColourCombination colourCombination, bool isPreview);
    }
}
