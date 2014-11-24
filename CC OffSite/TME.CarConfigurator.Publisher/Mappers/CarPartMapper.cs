using System;
using System.Linq;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class CarPartMapper : ICarPartMapper
    {
        private readonly IAssetSetMapper _assetSetMapper;

        public CarPartMapper(IAssetSetMapper assetSetMapper)
        {
            if (assetSetMapper == null) throw new ArgumentNullException("assetSetMapper");
            _assetSetMapper = assetSetMapper;
        }

        public CarPart MapCarPart(Administration.ModelGenerationCarPart generationCarPart)
        {
            return new CarPart
            {
                Code = generationCarPart.Code,
                Name = generationCarPart.Name,
                ID = generationCarPart.ID,
                VisibleIn = _assetSetMapper.GetVisibility(generationCarPart.AssetSet, true).ToList()
            };
        }
    }
}