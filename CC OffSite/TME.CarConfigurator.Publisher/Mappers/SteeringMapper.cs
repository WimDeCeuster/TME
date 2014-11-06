using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class SteeringMapper : ISteeringMapper
    {
        IBaseMapper _baseMapper;

        public SteeringMapper(IBaseMapper baseMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");

            _baseMapper = baseMapper;
        }

        public Steering MapSteering(Administration.Steering steering)
        {
            var mappedSteering = new Steering
            {
                SortIndex = 0
            };

            return _baseMapper.MapDefaults(mappedSteering, steering);
        }
    }
}
