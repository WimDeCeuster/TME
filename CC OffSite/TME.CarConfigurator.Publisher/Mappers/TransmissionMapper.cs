using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class TransmissionMapper : ITransmissionMapper
    {
        IBaseMapper _baseMapper;
        ITransmissionTypeMapper _transmissionTypeMapper;
        IAssetSetMapper _assetSetMapper;

        public TransmissionMapper(IBaseMapper baseMapper, ITransmissionTypeMapper transmissionTypeMapper, IAssetSetMapper assetSetMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");
            if (transmissionTypeMapper == null) throw new ArgumentNullException("transmissionTypeMapper");
            if (assetSetMapper == null) throw new ArgumentNullException("assetSetMapper");


            _baseMapper = baseMapper;
            _transmissionTypeMapper = transmissionTypeMapper;
            _assetSetMapper = assetSetMapper;
        }

        public Transmission MapTransmission(Administration.ModelGenerationTransmission generationTransmission)
        {
            var crossModelTransmission = Administration.Transmissions.GetTransmissions()[generationTransmission.ID];
            var transmissionType = Administration.TransmissionTypes.GetTransmissionTypes()[generationTransmission.Type.ID];

            var mappedTransmission = new Transmission
            {
                Brochure = generationTransmission.Brochure,
                KeyFeature = generationTransmission.KeyFeature,
                NumberOfGears = generationTransmission.NumberOfGears,
                Type = _transmissionTypeMapper.MapTransmissionType(transmissionType),
                VisibleIn = _assetSetMapper.GetVisibility(generationTransmission.AssetSet).ToList()
            };

            return _baseMapper.MapDefaultsWithSort(mappedTransmission, crossModelTransmission, generationTransmission);
        }
    }
}
