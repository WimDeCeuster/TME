using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;
using Link = TME.CarConfigurator.Repository.Objects.Link;
using Model = TME.CarConfigurator.Administration.Model;

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

        public SubModel MapSubModel(Model model, ModelGenerationSubModel modelGenerationSubModel, TimeFrame timeFrame, bool isPreview)
        {
            var cheapestCar = GetTheCheapestCar(modelGenerationSubModel, timeFrame.Cars);

            var mappedSubModel = new SubModel
            {
                StartingPrice = new Price
                {
                    ExcludingVat = cheapestCar.StartingPrice.ExcludingVat,
                    IncludingVat = cheapestCar.StartingPrice.IncludingVat,
                },
                Assets = GetMappedAssetsForSubModel(modelGenerationSubModel),
                Links = GetMappedLinksForSubModel(model, modelGenerationSubModel, isPreview),
                Grades = GetSubModelGrades(modelGenerationSubModel, timeFrame)
            };

            return _baseMapper.MapDefaultsWithSort(mappedSubModel, modelGenerationSubModel);
        }
        
        private IList<Grade> GetSubModelGrades(ModelGenerationSubModel modelGenerationSubModel, TimeFrame timeFrame)
        {
            return timeFrame.SubModelGrades[modelGenerationSubModel.ID].ToList();
        }

        private IList<Link> GetMappedLinksForSubModel(Model model, ModelGenerationSubModel modelGenerationSubModel, bool isPreview)
        {
            var generation = modelGenerationSubModel.Generation;

            var modelLinksNotAvailableForSubmodel = model.Links.Where(x => !modelGenerationSubModel.Links.Contains(x.Type));
            var mappedModelLinks = _linkMapper.MapLinks(generation, modelLinksNotAvailableForSubmodel, isPreview);
            var mappedSubModelLinks = _linkMapper.MapLinks(generation, modelGenerationSubModel.Links, isPreview);
            
            mappedSubModelLinks.AddRange(mappedModelLinks);
            return mappedSubModelLinks.OrderBy(x=>x.Name).ToList();

        }



        private IList<Asset> GetMappedAssetsForSubModel(ModelGenerationSubModel modelGenerationSubModel)
        {
            return modelGenerationSubModel.AssetSet.Assets
                .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGenerationSubModel.Generation))
                .ToList();
        }

        private static Repository.Objects.Car GetTheCheapestCar(ModelGenerationSubModel modelGenerationSubModel, IEnumerable<Repository.Objects.Car> cars)
        {
            return cars.Where(car => modelGenerationSubModel.Cars()
                .Any(subModelCar => subModelCar.ID == car.ID))
                .OrderBy(car => car.StartingPrice.IncludingVat)
                .First();
        }
    }
}