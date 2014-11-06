using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces.Equipment
{
    public interface IExteriorColour : IBaseObject
    {
        IColourTransformation Transformation { get; }
    }
}
