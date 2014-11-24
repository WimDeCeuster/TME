using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Repository.Objects.Colours;
using ExteriorColour = TME.CarConfigurator.Repository.Objects.Colours.ExteriorColour;
using ExteriorColourInfo = TME.CarConfigurator.Repository.Objects.Colours.ExteriorColourInfo;
using ExteriorColourType = TME.CarConfigurator.Administration.ExteriorColourType;
using UpholsteryInfo = TME.CarConfigurator.Repository.Objects.Colours.UpholsteryInfo;
using UpholsteryType = TME.CarConfigurator.Administration.UpholsteryType;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IColourMapper
    {
        ExteriorColour MapExteriorColour(ModelGeneration modelGeneration, ModelGenerationExteriorColour colour, bool isPreview, Administration.ExteriorColourType exteriorColourType, string assetUrl);
        ExteriorColour MapExteriorColour(ModelGeneration modelGeneration, Administration.ExteriorColour crossModelColour, bool isPreview, Administration.ExteriorColourType exteriorColourType, string assetUrl);
        ColourCombination MapColourCombination(ModelGeneration modelGeneration, ModelGenerationColourCombination colourCombination, bool isPreview, Administration.ExteriorColourType exteriorColourType, Administration.UpholsteryType upholsteryType, string assetUrl);
        ExteriorColourInfo MapExteriorColourInfo(Administration.ExteriorColourInfo colour);
        ExteriorColourInfo MapExteriorColourInfo(CarPackExteriorColour exteriorColour);
        ExteriorColourInfo MapExteriorColourApplicability(ExteriorColourApplicability applicability);
        UpholsteryInfo MapUpholsteryApplicability(UpholsteryApplicability applicability);
        UpholsteryInfo MapUpholsteryInfo(Administration.UpholsteryInfo upholstery);
        UpholsteryInfo MapUpholsteryInfo(CarPackUpholstery upholstery);
        CarColourCombination MapLinkedColourCombination(ModelGeneration modelGeneration, LinkedColourCombination carColourCombination, ExteriorColourType exteriorColourType, UpholsteryType upholsteryType, bool isPreview, string assetUrl);
    }
}
