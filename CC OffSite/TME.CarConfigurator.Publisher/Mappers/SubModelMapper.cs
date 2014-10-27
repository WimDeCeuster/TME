﻿using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;
using Link = TME.CarConfigurator.Repository.Objects.Link;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class SubModelMapper : ISubModelMapper
    {
        private readonly IBaseMapper _baseMapper;
        private readonly IAssetMapper _assetMapper;
        private readonly ILinkMapper _linkMapper;
        private readonly IGradeMapper _gradeMapper;

        public SubModelMapper(IBaseMapper baseMapper, IAssetMapper assetMapper, ILinkMapper linkMapper,IGradeMapper gradeMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");
            if (assetMapper == null) throw new ArgumentNullException("assetMapper");
            if (linkMapper == null) throw new ArgumentNullException("linkMapper");
            if (gradeMapper == null) throw new ArgumentNullException("gradeMapper");

            _baseMapper = baseMapper;
            _assetMapper = assetMapper;
            _linkMapper = linkMapper;
            _gradeMapper = gradeMapper;
        }

        public SubModel MapSubModel(ModelGenerationSubModel modelGenerationSubModel, ContextData contextData, bool isPreview)
        {
            var cheapestCar = GetTheCheapestCar(modelGenerationSubModel, contextData.Cars);

            var mappedSubModel = new SubModel
            {
                StartingPrice = new Price
                {
                    ExcludingVat = cheapestCar.StartingPrice.ExcludingVat,
                    IncludingVat = cheapestCar.StartingPrice.IncludingVat,
                },
                Assets = GetMappedAssetsForSubModel(modelGenerationSubModel),
                Links = GetMappedLinksForSubModel(modelGenerationSubModel, isPreview),
                Grades = GetSubModelGrades(modelGenerationSubModel, contextData)
            };

            return _baseMapper.MapDefaultsWithSort(mappedSubModel, modelGenerationSubModel, modelGenerationSubModel, modelGenerationSubModel.Name);
        }

        private List<Grade> GetSubModelGrades(ModelGenerationSubModel modelGenerationSubModel, ContextData contextData)
        {
            return contextData.Grades
                .Where(contextGrade => modelGenerationSubModel.Cars()
                                                              .Any(car => car.GradeID == contextGrade.ID))
                .ToList();
        }

        private List<Link> GetMappedLinksForSubModel(ModelGenerationSubModel modelGenerationSubModel, bool isPreview)
        {
            return modelGenerationSubModel.Links
                .Where(link => link.IsApplicableFor(modelGenerationSubModel.Generation))
                .Select(link => _linkMapper.MapLink(link, isPreview))
                .ToList();
        }

        private List<Asset> GetMappedAssetsForSubModel(ModelGenerationSubModel modelGenerationSubModel)
        {
            return modelGenerationSubModel.AssetSet.Assets
                .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGenerationSubModel.Generation))
                .ToList();
        }

        private static Repository.Objects.Car GetTheCheapestCar(ModelGenerationSubModel modelGenerationSubModel, IEnumerable<Repository.Objects.Car> cars)
        {
            return cars.Where(car => modelGenerationSubModel.Cars()
                .Any(subModelCar => subModelCar.ID == car.ID))
                .OrderBy(car => car.StartingPrice.ExcludingVat)
                .First();
        }
    }
}