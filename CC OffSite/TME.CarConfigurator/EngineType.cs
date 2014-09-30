using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator
{
    public class EngineType : IEngineType
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public IFuelType FuelType { get; private set; }
    }
}