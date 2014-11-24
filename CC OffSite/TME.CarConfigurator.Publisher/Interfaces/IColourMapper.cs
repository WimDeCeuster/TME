using System;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Repository.Objects.Colours;
using ExteriorColourInfo = TME.CarConfigurator.Repository.Objects.Colours.ExteriorColourInfo;
using UpholsteryInfo = TME.CarConfigurator.Repository.Objects.Colours.UpholsteryInfo;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IColourMapper
    {
        Repository.Objects.Equipment.ExteriorColour MapEquipmentExteriorColour(ModelGeneration modelGeneration, ModelGenerationExteriorColour colour, bool isPreview, String assetUrl);
        Repository.Objects.Equipment.ExteriorColour MapEquipmentExteriorColour(ModelGeneration modelGeneration,Administration.ExteriorColour colour, bool isPreview, String assetUrl);
        
        ColourCombination MapColourCombination(ModelGeneration modelGeneration, ModelGenerationColourCombination colourCombination, bool isPreview, Administration.ExteriorColourType exteriorColourType, Administration.UpholsteryType upholsteryType, string assetUrl);
        ExteriorColourInfo MapExteriorColourInfo(Administration.ExteriorColourInfo colour);
        ExteriorColourInfo MapExteriorColourInfo(CarPackExteriorColour exteriorColour);
        ExteriorColourInfo MapExteriorColourApplicability(ExteriorColourApplicability applicability);
        UpholsteryInfo MapUpholsteryApplicability(UpholsteryApplicability applicability);
        UpholsteryInfo MapUpholsteryInfo(Administration.UpholsteryInfo upholstery);
        UpholsteryInfo MapUpholsteryInfo(CarPackUpholstery upholstery);
    }
}
