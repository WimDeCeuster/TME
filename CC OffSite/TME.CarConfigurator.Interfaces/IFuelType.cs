using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces
{
    public interface IFuelType : IBaseObject
    {
        bool Hybrid { get; }
    }
}
