using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Repository.Objects.Equipment;
using Link = TME.CarConfigurator.Repository.Objects.Link;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class SubModelMapper : ISubModelMapper
    {
        private readonly IBaseMapper _baseMapper;
        private readonly IAssetMapper _assetMapper;
        private readonly ILinkMapper _linkMapper;
        private readonly IEquipmentMapper _equipmentMapper;

        public SubModelMapper(IBaseMapper baseMapper, IAssetMapper assetMapper, ILinkMapper linkMapper,IEquipmentMapper equipmentMapper)
        {
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");
            if (assetMapper == null) throw new ArgumentNullException("assetMapper");
            if (linkMapper == null) throw new ArgumentNullException("linkMapper");
            if (equipmentMapper == null) throw new ArgumentNullException("equipmentMapper");

            _baseMapper = baseMapper;
            _assetMapper = assetMapper;
            _linkMapper = linkMapper;
            _equipmentMapper = equipmentMapper;
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
                Grades = GetSubModelGrades(modelGenerationSubModel, contextData),
                Equipment = GetSubModelEquipment(modelGenerationSubModel,contextData)
            };

            return _baseMapper.MapDefaultsWithSort(mappedSubModel, modelGenerationSubModel, modelGenerationSubModel, modelGenerationSubModel.Name);
        }

        private List<GradeEquipmentItem> GetSubModelEquipment(ModelGenerationSubModel modelGenerationSubModel, ContextData contextData)
        {
            var accesories =  contextData.GradeEquipments.Values.SelectMany(e => e.Accessories).ToList().Where(a => modelGenerationSubModel.Equipment.Any(aq => aq.ID == a.ID));
            return accesories.Select(gradeAccessory => new GradeAccessory()
            {
                BestVisibleIn = gradeAccessory.BestVisibleIn, Category = gradeAccessory.Category, Description = gradeAccessory.Description, ExteriorColour = gradeAccessory.ExteriorColour, FootNote = gradeAccessory.FootNote, GradeFeature = gradeAccessory.GradeFeature, ID = gradeAccessory.ID, InternalCode = gradeAccessory.InternalCode, InternalName = gradeAccessory.InternalName, KeyFeature = gradeAccessory.KeyFeature, Labels = gradeAccessory.Labels, Links = gradeAccessory.Links, LocalCode = gradeAccessory.LocalCode, SortIndex = gradeAccessory.SortIndex, ToolTip = gradeAccessory.ToolTip, Name = gradeAccessory.Name, NotAvailable = gradeAccessory.NotAvailable, NotAvailableOn = gradeAccessory.NotAvailableOn, Optional = gradeAccessory.Optional, OptionalGradeFeature = gradeAccessory.OptionalGradeFeature, OptionalOn = gradeAccessory.OptionalOn, PartNumber = gradeAccessory.PartNumber, Path = gradeAccessory.Path, ShortID = gradeAccessory.ShortID, Standard = gradeAccessory.Standard, StandardOn = gradeAccessory.StandardOn, Visibility = gradeAccessory.Visibility
            }).Cast<GradeEquipmentItem>().ToList();
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