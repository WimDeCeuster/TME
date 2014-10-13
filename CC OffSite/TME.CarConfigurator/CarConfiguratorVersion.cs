using System;
using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator
{
    public class CarConfiguratorVersion : ICarConfiguratorVersion
    {
        private readonly Repository.Objects.CarConfiguratorVersion _carConfiguratorVersion;

        public CarConfiguratorVersion(Repository.Objects.CarConfiguratorVersion carConfiguratorVersion)
        {
            if (carConfiguratorVersion == null) throw new ArgumentNullException("carConfiguratorVersion");
            _carConfiguratorVersion = carConfiguratorVersion;
        }

        public short ID { get { return _carConfiguratorVersion.ID; } }
        public string Name { get { return _carConfiguratorVersion.Name; } }
    }
}