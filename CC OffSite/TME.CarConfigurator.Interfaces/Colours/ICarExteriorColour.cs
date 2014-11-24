using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces.Colours
{
    public interface ICarExteriorColour : IExteriorColour
    {
        IPrice Price { get; }
    }
}