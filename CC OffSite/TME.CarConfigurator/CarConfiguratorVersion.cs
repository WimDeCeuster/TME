using System;
using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator
{
    public class CarConfiguratorVersion : ICarConfiguratorVersion
    {
        private readonly Repository.Objects.CarConfiguratorVersion _repositoryCarConfiguratorVersion;

        public CarConfiguratorVersion(Repository.Objects.CarConfiguratorVersion repositoryCarConfiguratorVersion)
        {
            if (repositoryCarConfiguratorVersion == null) throw new ArgumentNullException("repositoryCarConfiguratorVersion");
            _repositoryCarConfiguratorVersion = repositoryCarConfiguratorVersion;
        }

        public short ID { get { return _repositoryCarConfiguratorVersion.ID; } }
        public string Name { get { return _repositoryCarConfiguratorVersion.Name; } }
    }
}