using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class EngineTypeMapper : IEngineTypeMapper
    {
        readonly IFuelTypeMapper _fuelTypeMapper;

        public EngineTypeMapper(IFuelTypeMapper fuelTypeMapper)
        {
            if (fuelTypeMapper == null) throw new ArgumentNullException("fuelTypeMapper");

            _fuelTypeMapper = fuelTypeMapper;
        }

        public EngineType MapEngineType(Administration.EngineTypeInfo engineTypeInfo)
        {
            var fuelType = Administration.FuelTypes.GetFuelTypes()[engineTypeInfo.FuelType.ID];

            return new EngineType
            {
                Code = engineTypeInfo.Code,
                FuelType = _fuelTypeMapper.MapFuelType(fuelType),
                Name = engineTypeInfo.Name
            };
        }
    }
}
