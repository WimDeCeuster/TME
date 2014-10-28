using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IColourMapper
    {
        ExteriorColour MapExteriorColour(Administration.ModelGenerationExteriorColour colour, string colourFilePath);

        ExteriorColour MapExteriorColour(Administration.ExteriorColour crossModelColour, string colourFilePath);
    }
}
