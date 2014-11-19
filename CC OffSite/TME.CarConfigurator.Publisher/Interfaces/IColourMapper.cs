using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Repository.Objects.Colours;
using ExteriorColour = TME.CarConfigurator.Repository.Objects.Colours.ExteriorColour;
using ExteriorColourInfo = TME.CarConfigurator.Repository.Objects.Colours.ExteriorColourInfo;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IColourMapper
    {
        ExteriorColour MapExteriorColour(ModelGeneration modelGeneration, ModelGenerationExteriorColour colour, bool isPreview, Administration.ExteriorColourType exteriorColourType);
        ExteriorColour MapExteriorColour(ModelGeneration modelGeneration, Administration.ExteriorColour crossModelColour, bool isPreview, Administration.ExteriorColourType exteriorColourType);
        ColourCombination MapColourCombination(ModelGeneration modelGeneration, ModelGenerationColourCombination colourCombination, bool isPreview, Administration.ExteriorColourType exteriorColourType, Administration.UpholsteryType upholsteryType);
        ExteriorColourInfo MapExteriorColourInfo(Administration.ExteriorColourInfo colour);
    }
}
