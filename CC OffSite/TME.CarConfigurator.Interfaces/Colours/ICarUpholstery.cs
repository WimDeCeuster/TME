using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces.Colours
{
    public interface ICarUpholstery : IUpholstery
    {
        IPrice Price { get; }
    }
}