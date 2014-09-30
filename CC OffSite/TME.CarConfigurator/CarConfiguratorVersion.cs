using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator
{
    public class CarConfiguratorVersion : ICarConfiguratorVersion
    {
        public short ID { get; private set; }
        public string Name { get; private set; }
    }
}