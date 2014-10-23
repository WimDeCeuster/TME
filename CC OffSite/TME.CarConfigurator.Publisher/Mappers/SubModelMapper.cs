using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;
using Car = TME.CarConfigurator.Administration.Car;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class SubModelMapper : ISubModelMapper
    {
        private readonly IBaseMapper _baseMapper;
        private readonly IAssetMapper _assetMapper;
        private readonly ILinkMapper _linkMapper;

        public SubModelMapper(IBaseMapper baseMapper, IAssetMapper assetMapper, ILinkMapper linkMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");
            if (assetMapper == null) throw new ArgumentNullException("assetMapper");
            if (linkMapper == null) throw new ArgumentNullException("linkMapper");

            _baseMapper = baseMapper;
            _assetMapper = assetMapper;
            _linkMapper = linkMapper;
        }

        public SubModel MapSubModel(ModelGenerationSubModel modelGenerationSubModel, IEnumerable<Car> cars, bool isPreview)
        {
            var subModelCars = modelGenerationSubModel.Cars();

            var cheapestCar = cars.Where(car => subModelCars.Any(subModelCar => subModelCar.ID == car.ID))
                .OrderBy(car => car.Price)
                .First();

            var mappedSubModel = new SubModel()
            {
                StartingPrice = new Price()
                {
                    ExcludingVat = cheapestCar.Price,
                    IncludingVat = cheapestCar.VatPrice,
                },
                Assets = modelGenerationSubModel.AssetSet.Assets.Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGenerationSubModel.Generation)).ToList(),
                Links = modelGenerationSubModel.Links.Where(link => link.IsApplicableFor(modelGenerationSubModel.Generation))
                .Select(link => _linkMapper.MapLink(link ,isPreview)).ToList()
            };

            return _baseMapper.MapDefaultsWithSort(mappedSubModel, modelGenerationSubModel, modelGenerationSubModel, modelGenerationSubModel.Name);
        }

        
    }
}