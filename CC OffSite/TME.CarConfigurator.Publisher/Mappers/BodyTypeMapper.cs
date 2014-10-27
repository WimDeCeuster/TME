using System;
using System.Linq;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class BodyTypeMapper : IBodyTypeMapper
    {
        readonly IBaseMapper _baseMapper;
        readonly IAssetSetMapper _assetSetMapper;

        public BodyTypeMapper(IBaseMapper baseMapper, IAssetSetMapper assetSetMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");
            if (assetSetMapper == null) throw new ArgumentNullException("assetSetMapper");

            _baseMapper = baseMapper;
            _assetSetMapper = assetSetMapper;
        }

        public BodyType MapBodyType(Administration.ModelGenerationBodyType generationBodyType)
        {
            var crossModelBodyType = Administration.BodyTypes.GetBodyTypes()[generationBodyType.ID];

            var mappedBodyType = new BodyType
            {
                NumberOfDoors = generationBodyType.NumberOfDoors,
                NumberOfSeats = generationBodyType.NumberOfSeats,
                VisibleIn = _assetSetMapper.GetVisibility(generationBodyType.AssetSet).ToList()
            };

            return _baseMapper.MapDefaultsWithSort(mappedBodyType, crossModelBodyType, generationBodyType, generationBodyType.Name);
        }
    }
}
