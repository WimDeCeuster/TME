using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class FuelTypeMapper : IFuelTypeMapper
    {
        readonly IBaseMapper _baseMapper;

        public FuelTypeMapper(IBaseMapper baseMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");

            _baseMapper = baseMapper;
        }

        public FuelType MapFuelType(Administration.FuelType fuelType)
        {
            var mappedFuelType = new FuelType
            {
                Hybrid = fuelType.Code.ToUpper(System.Globalization.CultureInfo.InvariantCulture).StartsWith("H"),
                SortIndex = 0
            };

            return _baseMapper.MapDefaults(mappedFuelType, fuelType);
        }
    }
}
