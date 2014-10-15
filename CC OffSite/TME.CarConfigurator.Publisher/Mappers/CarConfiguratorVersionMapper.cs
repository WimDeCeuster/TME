using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class CarConfiguratorVersionMapper : ICarConfiguratorVersionMapper
    {
        public CarConfiguratorVersion MapCarConfiguratorVersion(Administration.ModelGenerationCarConfiguratorVersion version)
        {
            return new CarConfiguratorVersion {
                ID = version.ID,
                Name = version.Name
            };
        }
    }
}
