using System;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class TransmissionTypeMapper : ITransmissionTypeMapper
    {
        readonly IBaseMapper _baseMapper;

        public TransmissionTypeMapper(IBaseMapper baseMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");

            _baseMapper = baseMapper;
        }

        public TransmissionType MapTransmissionType(Administration.TransmissionType transmissionType)
        {
            var mappedTransmissionType = new TransmissionType
            {
                SortIndex = 0,
            };

            return _baseMapper.MapDefaults(mappedTransmissionType, transmissionType, transmissionType);
        }
    }
}
