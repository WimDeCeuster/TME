﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Enums;
using TME.CarConfigurator.Administration.Interfaces;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.Progress;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Extensions;
using AccentColourCombination = TME.CarConfigurator.Repository.Objects.Packs.AccentColourCombination;
using Asset = TME.CarConfigurator.Repository.Objects.Assets.Asset;
using GradePack = TME.CarConfigurator.Repository.Objects.Packs.GradePack;
using Car = TME.CarConfigurator.Administration.Car;
using CarAccessory = TME.CarConfigurator.Administration.CarAccessory;
using CarEquipment = TME.CarConfigurator.Repository.Objects.Equipment.CarEquipment;
using CarOption = TME.CarConfigurator.Administration.CarOption;
using Model = TME.CarConfigurator.Administration.Model;

namespace TME.CarConfigurator.Publisher
{
    public class Mapper : IMapper
    {
        readonly ITimeFrameMapper _timeFrameMapper;
        readonly IModelMapper _modelMapper;
        readonly IGenerationMapper _generationMapper;
        readonly IBodyTypeMapper _bodyTypeMapper;
        readonly IEngineMapper _engineMapper;
        readonly ITransmissionMapper _transmissionMapper;
        readonly IWheelDriveMapper _wheelDriveMapper;
        readonly IGradeMapper _gradeMapper;
        readonly ISteeringMapper _steeringMapper;
        readonly ICarMapper _carMapper;
        readonly IAssetMapper _assetMapper;
        readonly ISubModelMapper _subModelMapper;
        readonly IEquipmentMapper _equipmentMapper;
        readonly IPackMapper _packMapper;
        readonly IColourMapper _colourMapper;
        readonly ICategoryMapper _categoryMapper;
        readonly ICarPartMapper _carPartMapper;
        readonly ITechnicalSpecificationMapper _technicalSpecificationMapper;
        readonly IRuleMapper _ruleMapper;

        public Mapper(
            ITimeFrameMapper timeFrameMapper,
            IModelMapper modelMapper,
            IGenerationMapper generationMapper,
            IBodyTypeMapper bodyTypeMapper,
            IEngineMapper engineMapper,
            ITransmissionMapper transmissionMapper,
            IWheelDriveMapper wheelDriveMapper,
            ISteeringMapper steeringMapper,
            IGradeMapper gradeMapper,
            ICarMapper carMapper,
            IAssetMapper assetMapper,
            ISubModelMapper subModelMapper,
            IEquipmentMapper equipmentMapper,
            IPackMapper packMapper,
            IColourMapper colourMapper,
            ICategoryMapper categoryMapper,
            ICarPartMapper carPartMapper,
            ITechnicalSpecificationMapper technicalSpecificationMapper, 
            IRuleMapper ruleMapper)
        {
            if (timeFrameMapper == null) throw new ArgumentNullException("timeFrameMapper");
            if (modelMapper == null) throw new ArgumentNullException("modelMapper");
            if (generationMapper == null) throw new ArgumentNullException("generationMapper");
            if (bodyTypeMapper == null) throw new ArgumentNullException("bodyTypeMapper");
            if (engineMapper == null) throw new ArgumentNullException("engineMapper");
            if (transmissionMapper == null) throw new ArgumentNullException("transmissionMapper");
            if (wheelDriveMapper == null) throw new ArgumentNullException("wheelDriveMapper");
            if (steeringMapper == null) throw new ArgumentNullException("steeringMapper");
            if (gradeMapper == null) throw new ArgumentNullException("gradeMapper");
            if (carMapper == null) throw new ArgumentNullException("carMapper");
            if (assetMapper == null) throw new ArgumentNullException("assetMapper");
            if (subModelMapper == null) throw new ArgumentNullException("subModelMapper");
            if (equipmentMapper == null) throw new ArgumentNullException("equipmentMapper");
            if (packMapper == null) throw new ArgumentNullException("packMapper");
            if (categoryMapper == null) throw new ArgumentNullException("categoryMapper");
            if (carPartMapper == null) throw new ArgumentNullException("carPartMapper");
            if (technicalSpecificationMapper == null) throw new ArgumentNullException("technicalSpecificationMapper");
            if (ruleMapper == null) throw new ArgumentNullException("ruleMapper");

            _timeFrameMapper = timeFrameMapper;
            _modelMapper = modelMapper;
            _assetMapper = assetMapper;
            _subModelMapper = subModelMapper;
            _generationMapper = generationMapper;
            _bodyTypeMapper = bodyTypeMapper;
            _engineMapper = engineMapper;
            _transmissionMapper = transmissionMapper;
            _wheelDriveMapper = wheelDriveMapper;
            _steeringMapper = steeringMapper;
            _gradeMapper = gradeMapper;
            _carMapper = carMapper;
            _equipmentMapper = equipmentMapper;
            _packMapper = packMapper;
            _colourMapper = colourMapper;
            _categoryMapper = categoryMapper;
            _carPartMapper = carPartMapper;
            _technicalSpecificationMapper = technicalSpecificationMapper;
            _ruleMapper = ruleMapper;
        }

        public Task MapAsync(IContext context, IProgress<PublishProgress> progress)
        {
            return Task.Run(() => Map(context, progress));
        }

        private void Map(IContext context, IProgress<PublishProgress> progress)
        {
            progress.Report(new PublishProgress("Retrieving generation for every language"));

            var data = GetModelGenerationForEachLanguage(context.Brand, context.Country, context.GenerationID);
            var isPreview = context.DataSubset == PublicationDataSubset.Preview;

            foreach (var entry in data)
            {
                var contextData = new ContextData();
                var modelGeneration = entry.Value.Item1;
                var model = entry.Value.Item2;
                var language = entry.Key;

                MyContext.SetSystemContext(context.Brand, context.Country, language);
                MyContext.GetContext().LazyLoad = false;

                context.ModelGenerations[language] = modelGeneration;
                context.ContextData[language] = contextData;

                // fill contextData
                var equipmentItems = Administration.EquipmentItems.GetEquipmentItems();
                var equipmentGroups = MyContext.GetContext().EquipmentGroups;
                var equipmentCategories = EquipmentCategories.GetEquipmentCategories(true);
                var specificationCategories = SpecificationCategories.GetSpecificationCategories(true);
                var units = Units.GetUnits();
                var exteriorColourTypes = ExteriorColourTypes.GetExteriorColourTypes();
                var upholsteryTypes = UpholsteryTypes.GetUpholsteryTypes();
                
                progress.Report(new PublishProgress(String.Format("Fill context data for language {0}", language)));

                progress.Report(new PublishProgress("Map generation"));
                contextData.Generations.Add(_generationMapper.MapGeneration(model, modelGeneration, isPreview));
                progress.Report(new PublishProgress("Map model"));
                contextData.Models.Add(_modelMapper.MapModel(model));
                progress.Report(new PublishProgress("Get generation cars"));
                var cars = modelGeneration.Cars.Where(car => isPreview ? car.Preview : car.Approved).ToList();

                progress.Report(new PublishProgress("Calculate publication timeframes"));
                context.TimeFrames[language] = _timeFrameMapper.GetTimeFrames(language, context);

                progress.Report(new PublishProgress("Get generation grades"));
                var grades = modelGeneration.Grades.Where(grade => cars.Any(car => car.GradeID == grade.ID)).ToArray();

                progress.Report(new PublishProgress("Fill generation assets"));
                FillAssets(modelGeneration, contextData);

                progress.Report(new PublishProgress("Fill generation equipment categories"));
                FillEquipmentCategories(equipmentCategories, contextData);
                progress.Report(new PublishProgress("Fill generation specification categories"));
                FillSpecificationCategories(specificationCategories, contextData);

                progress.Report(new PublishProgress("Fill timeframes"));
                foreach (var timeFrame in context.TimeFrames[language])
                {
                    var timeFrameCars = cars.Where(car => timeFrame.TimeFrameCarIds.Contains(car.ID)).ToList();
                    var timeFrameGrades = grades.Where(grade => grade.Cars().Any(timeFrameCars.Contains)).ToList();

                    progress.Report(new PublishProgress("Fill generation body types"));
                    FillBodyTypes(timeFrameCars, modelGeneration, timeFrame);
                    progress.Report(new PublishProgress("Fill generation engines"));
                    FillEngines(timeFrameCars, modelGeneration, timeFrame);
                    progress.Report(new PublishProgress("Fill generation transmissions"));
                    FillTransmissions(timeFrameCars, modelGeneration, timeFrame);
                    progress.Report(new PublishProgress("Fill generation wheeldrives"));
                    FillWheelDrives(timeFrameCars, modelGeneration, timeFrame);
                    progress.Report(new PublishProgress("Fill generation steerings"));
                    FillSteerings(timeFrameCars, timeFrame);
                    progress.Report(new PublishProgress("Fill generation cars"));
                    FillCars(timeFrameCars, timeFrame, isPreview);
                    
                    progress.Report(new PublishProgress("Fill generation grades"));
                    FillGrades(timeFrameCars, modelGeneration, timeFrame);
                    progress.Report(new PublishProgress("Fill generation grade packs"));
                    FillGradePacks(timeFrameGrades, timeFrame, isPreview);
                    progress.Report(new PublishProgress("Fill grade equipment"));
                    FillGradeEquipment(equipmentCategories, equipmentGroups, timeFrameCars, timeFrameGrades, timeFrame, isPreview, exteriorColourTypes, context.AssetUrl);
                    progress.Report(new PublishProgress("Fill generation submodels"));
                    FillSubModels(model, timeFrameGrades, timeFrameCars, modelGeneration, timeFrame, isPreview, equipmentCategories, equipmentGroups, exteriorColourTypes, context.AssetUrl);
                    progress.Report(new PublishProgress("Fill submodel grade packs"));
                    FillSubModelGradePacks(timeFrameGrades, modelGeneration, timeFrame, isPreview);
                    progress.Report(new PublishProgress("Fill generation colour combinations"));
                    FillColourCombinations(timeFrameCars, modelGeneration, timeFrame, isPreview, exteriorColourTypes, upholsteryTypes, context.AssetUrl);
                }

                progress.Report(new PublishProgress("Fill submodel assets"));
                FillSubModelAssets(grades, modelGeneration, contextData);
                progress.Report(new PublishProgress("Fill generation car assets"));
                FillCarAssets(cars, contextData, modelGeneration);

                progress.Report(new PublishProgress("Fill generation car items (Parts/Packs/Equipment/TechSpec/ColourCombinations)"));
                foreach (var car in cars)
                {
                    FillCarParts(car, contextData);
                    FillCarPacks(car, contextData, equipmentGroups, equipmentCategories, isPreview, context.AssetUrl);
                    FillCarPackAccentColourCombination(contextData, car, modelGeneration, isPreview, context.AssetUrl);
                    FillCarEquipment(car, contextData, equipmentCategories, equipmentGroups, isPreview, context.AssetUrl);
                    FillCarTechnicalSpecifications(car, specificationCategories, units, contextData);
                    FillCarRules(car, contextData, equipmentGroups, equipmentItems);
                    FillCarColourCombinations(car, contextData, exteriorColourTypes, upholsteryTypes, isPreview, context.AssetUrl);
                }
                progress.Report(new PublishProgress("Mapping complete"));
            }
        }

        private static IEnumerable<KeyValuePair<string, Tuple<ModelGeneration, Model>>> GetModelGenerationForEachLanguage(String brand, String countryCode, Guid generationID)
        {
            var country = MyContext.GetContext().Countries.Single(ctry => ctry.Code.Equals(countryCode, StringComparison.InvariantCultureIgnoreCase));
            return country.Languages.ToDictionary(lang => lang.Code, lang =>
            {
                MyContext.SetSystemContext(brand, countryCode, lang.Code);
                var generation = ModelGeneration.GetModelGeneration(generationID);
                var model = Model.GetModel(generation.Model.ID);

                return Tuple.Create(generation, model);
            });
        }

        private void FillAssets(ModelGeneration modelGeneration, ContextData contextData)
        {
            contextData.Assets =
                GetBodyTypeAssets(modelGeneration)
                .Concat(GetEngineAssets(modelGeneration))
                .Concat(GetTransmissionAssets(modelGeneration))
                .Concat(GetWheelDriveAssets(modelGeneration))
                .Concat(GetGradeAssets(modelGeneration))
                .Concat(GetSubModelAssets(modelGeneration))
                .Concat(GetColourCombinationAssets(modelGeneration))
                .ToDictionary(
                    entry => entry.Key,
                    entry => (IList<Asset>)entry.Value);
        }

        private void FillCarAssets(IEnumerable<Car> cars, ContextData contextData, ModelGeneration modelGeneration)
        {
            var carItemsGenerationAssets = new Dictionary<Guid, IList<Asset>>();
            var carItemAssets = new Dictionary<Guid, Asset>();

            foreach (var car in cars)
            {
                if (!contextData.CarAssets.ContainsKey(car.ID))
                    contextData.CarAssets.Add(car.ID, new Dictionary<Guid, IList<Asset>>());

                FillCarAssets(car, contextData, modelGeneration, car.Generation.BodyTypes[car.BodyTypeID]);
                FillCarAssets(car, contextData, modelGeneration, car.Generation.Engines[car.EngineID]);
                FillCarAssets(car, contextData, modelGeneration, car.Generation.Grades[car.GradeID]);
                FillCarAssets(car, contextData, modelGeneration, car.Generation.Transmissions[car.TransmissionID]);
                FillCarAssets(car, contextData, modelGeneration, car.Generation.WheelDrives[car.WheelDriveID]);
                FillCarAssets(car, contextData, modelGeneration, car.Generation.SubModels[car.SubModelID]);
                FillCarListAssets(car, contextData, GetValidCarPacks(car), carItemAssets, carItemsGenerationAssets);
                FillCarEquipmentAssets(car, contextData, GetValidCarEquipment(car), carItemAssets, carItemsGenerationAssets);
                FillCarColourCombinationAssets(car, contextData, carItemAssets, carItemsGenerationAssets);
                FillCarPartAssets(car, contextData, GetValidCarParts(car), carItemAssets, carItemsGenerationAssets);
            }
        }

        private void FillCarPartAssets(Car car, ContextData contextData, IEnumerable<ModelGenerationCarPart> carParts, Dictionary<Guid, Asset> carItemAssets, Dictionary<Guid, IList<Asset>> carItemsGenerationAssets)
        {
            contextData.CarPartAssets.Add(car.ID, carParts.ToDictionary(
                carPart => carPart.ID,
                carPart => GetCarItemAssets(carPart, car, carItemAssets, carItemsGenerationAssets)));
        }

        private void FillCarEquipmentAssets(Car car, ContextData contextData, IEnumerable<ModelGenerationEquipmentItem> equipmentItems, Dictionary<Guid, Asset> carItemAssets, Dictionary<Guid, IList<Asset>> carItemsGenerationAssets)
        {
            contextData.CarEquipmentAssets.Add(car.ID, equipmentItems.ToDictionary(
                equipmentItem => equipmentItem.ID,
                equipmentItem => GetCarItemAssets(equipmentItem, car, carItemAssets, carItemsGenerationAssets)));
        }

        private IList<Asset> GetCarItemAssets(IHasAssetSet objectWithAssetSet, Car car, Dictionary<Guid, Asset> carItemAssets, Dictionary<Guid, IList<Asset>> carItemsGenerationAssets)
        {
            var objectId = objectWithAssetSet.GetObjectID();
            var applicableAssets = objectWithAssetSet.AssetSet.Assets.Filter(car).ToList();

            if (objectWithAssetSet is ModelGenerationOption && ((ModelGenerationOption)objectWithAssetSet).Components.Count != 0)
                applicableAssets.AddRange(((ModelGenerationOption) objectWithAssetSet).Components.GetFilteredAssets(car));

            //var comparer = new Helpers.Comparer<Administration.Assets.AssetSetAsset>(x =>
            //    Tuple.Create(x.Asset.ID, x.AssetType.Code, x.BodyType.ID, x.Engine.ID, x.EquipmentItem.ID, x.ExteriorColour.ID, x.Grade.ID, Tuple.Create(x.Steering.ID, x.Transmission.ID, x.Upholstery.ID, x.WheelDrive.ID)));

            var comparer = new Helpers.Comparer<Administration.Assets.AssetSetAsset>(x =>
                String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}", x.Asset.ID, x.AssetType.Code, x.BodyType.ID, x.Engine.ID, x.EquipmentItem.ID, x.ExteriorColour.ID, x.Grade.ID, x.Steering.ID, x.Transmission.ID, x.Upholstery.ID, x.WheelDrive.ID));

            var filteredApplicableAssets = applicableAssets.Distinct(comparer).ToList();
            
            var unmappedApplicableAssets = filteredApplicableAssets.Where(asset => !carItemAssets.ContainsKey(asset.ID));

            foreach (var assetSetAsset in unmappedApplicableAssets)
                carItemAssets.Add(assetSetAsset.ID, _assetMapper.MapCarAssetSetAsset(assetSetAsset, car.Generation));

            if (!carItemsGenerationAssets.ContainsKey(objectId))
                carItemsGenerationAssets.Add(objectId, objectWithAssetSet.AssetSet.Assets.GetGenerationAssets().Select(asset => _assetMapper.MapCarAssetSetAsset(asset, car.Generation)).ToList());

            return filteredApplicableAssets.Select(asset => carItemAssets[asset.ID]).Distinct().Concat(carItemsGenerationAssets[objectId]).ToList();
        }

        private void FillCarAssets(Car car, ContextData contextData, ModelGeneration modelGeneration, IHasAssetSet objectWithAssetSet)
        {
            if (objectWithAssetSet == null) return;
            var objectAssetsOnCarLevel = GetObjectAssetsOnCarLevel(car, modelGeneration, objectWithAssetSet);
            var objectAssetsOnGenerationLevel = FindAssetsForObjectId(objectWithAssetSet.GetObjectID(), contextData.Assets);

            var allCarAssetsForThisObject = objectAssetsOnCarLevel.Concat(objectAssetsOnGenerationLevel).ToList();

            if (allCarAssetsForThisObject.Any())
                contextData.CarAssets[car.ID].Add(objectWithAssetSet.GetObjectID(), allCarAssetsForThisObject);
        }

        private void FillCarAssets(Car car, ContextData contextData, IHasAssetSet objectWithAssetSet, Dictionary<Guid, Asset> carItemAssets, Dictionary<Guid, IList<Asset>> carItemsGenerationAssets)
        {
            contextData.CarAssets[car.ID].Add(objectWithAssetSet.GetObjectID(), GetCarItemAssets(objectWithAssetSet, car, carItemAssets, carItemsGenerationAssets));
        }

        private void FillCarListAssets(Car car, ContextData contextData, IEnumerable<IHasAssetSet> itemsWithAssetSet, Dictionary<Guid, Asset> carItemAssets, Dictionary<Guid, IList<Asset>> carItemsGenerationAssets)
        {
            foreach (var item in itemsWithAssetSet)
            {
                FillCarAssets(car, contextData, item, carItemAssets, carItemsGenerationAssets);
            }
        }

        private void FillSubModelAssets(IList<ModelGenerationGrade> grades, ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var subModel in modelGeneration.SubModels)
            {
                FillSubModelGradeAssets(grades, subModel, contextData, modelGeneration);
            }
        }

        private void FillSubModelGradeAssets(IEnumerable<ModelGenerationGrade> grades, ModelGenerationSubModel subModel, ContextData contextData, ModelGeneration modelGeneration)
        {
            var assetsPerGrade = grades.Where(grade => grade.SubModels.Contains(subModel))
                                       .ToDictionary(grade => grade.ID,
                                                     grade => (IList<Asset>)grade.AssetSet.Assets.Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
            contextData.SubModelAssets.Add(subModel.ID, assetsPerGrade);
        }

        private IEnumerable<Asset> GetObjectAssetsOnCarLevel(Car car, ModelGeneration modelGeneration, IHasAssetSet objectWithAssetSet)
        {
            var objectAssetsOnCarLevel = objectWithAssetSet.AssetSet.Assets.Filter(car);

            return objectAssetsOnCarLevel.Select(asset => _assetMapper.MapCarAssetSetAsset(asset, modelGeneration)).ToList();
        }

        private static IEnumerable<Asset> FindAssetsForObjectId(Guid objectId, IDictionary<Guid, IList<Asset>> assets)
        {
            return assets.ContainsKey(objectId) ? assets[objectId] : new List<Asset>();
        }

        private IEnumerable<KeyValuePair<Guid, List<Asset>>> GetSubModelAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.SubModels.ToDictionary(
                subModel => subModel.ID,
                subModel => subModel.AssetSet.Assets.GetGenerationAssets()
                    .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private void FillCarColourCombinationAssets(Car car, ContextData contextData, Dictionary<Guid, Asset> carItemAssets, Dictionary<Guid, IList<Asset>> carItemsGenerationAssets)
        {
            FillCarListAssets(car, contextData, GetCarExteriorColours(car), carItemAssets, carItemsGenerationAssets);
            FillCarListAssets(car, contextData, GetCarUpholsteries(car), carItemAssets, carItemsGenerationAssets);
        }

        private IEnumerable<IHasAssetSet> GetCarUpholsteries(Car car)
        {
            return car.ColourCombinations.Upholsteries().Select(upholstery => upholstery.GenerationUpholstery);
        }

        private IEnumerable<IHasAssetSet> GetCarExteriorColours(Car car)
        {
            return car.ColourCombinations.ExteriorColours().Select(exCol => exCol.GenerationExteriorColour);
        }

        private IEnumerable<KeyValuePair<Guid, List<Asset>>> GetColourCombinationAssets(ModelGeneration modelGeneration)
        {
            return GetExteriorColourAssets(modelGeneration).Concat(GetUpholsteryAssets(modelGeneration));
        }

        private IEnumerable<KeyValuePair<Guid, List<Asset>>> GetExteriorColourAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.ColourCombinations
                                  .ExteriorColours()
                                  .ToDictionary(
                                    colour => colour.ID,
                                    colour => colour.AssetSet.Assets.GetGenerationAssets().GetExteriorColourAssets()
                                        .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private IEnumerable<KeyValuePair<Guid, List<Asset>>> GetUpholsteryAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.ColourCombinations
                                  .Upholsteries()
                                  .ToDictionary(
                                    upholstery => upholstery.ID,
                                    upholstery => upholstery.AssetSet.Assets.GetGenerationAssets().GetUpholsteryAssets()
                                        .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private IEnumerable<KeyValuePair<Guid, List<Asset>>> GetGradeAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.Grades.ToDictionary(
                grade => grade.ID,
                grade => grade.AssetSet.Assets.GetGenerationAssets()
                    .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private IEnumerable<KeyValuePair<Guid, List<Asset>>> GetTransmissionAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.Transmissions.ToDictionary(
                transmission => transmission.ID,
                transmission => transmission.AssetSet.Assets.GetGenerationAssets()
                        .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private IEnumerable<KeyValuePair<Guid, List<Asset>>> GetBodyTypeAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.BodyTypes.ToDictionary(
                bodytype => bodytype.ID,
                bodytype => bodytype.AssetSet.Assets.GetGenerationAssets()
                        .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private IEnumerable<KeyValuePair<Guid, List<Asset>>> GetEngineAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.Engines.ToDictionary(
                engine => engine.ID,
                engine => engine.AssetSet.Assets
                                         .GetGenerationAssets()
                                         .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private IEnumerable<KeyValuePair<Guid, List<Asset>>> GetWheelDriveAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.WheelDrives.ToDictionary(
                wheelDrive => wheelDrive.ID,
                wheelDrive => wheelDrive.AssetSet.Assets
                                         .GetGenerationAssets()
                                         .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        void FillCars(IEnumerable<Car> cars, TimeFrame timeFrame, bool isPreview)
        {
            timeFrame.Cars = cars.Select(car => {
                var bodyType = _carMapper.CopyBodyType(timeFrame.BodyTypes.Single(type => type.ID == car.BodyTypeID));
                var engine = _carMapper.CopyEngine(timeFrame.Engines.Single(eng => eng.ID == car.EngineID));
                var transmission = _carMapper.CopyTransmission(timeFrame.Transmissions.Single(trans => trans.ID == car.TransmissionID));
                var wheelDrive = _carMapper.CopyWheelDrive(timeFrame.WheelDrives.Single(drive => drive.ID == car.WheelDriveID));
                var steering = timeFrame.Steerings.Single(steer => steer.ID == car.SteeringID);
                return _carMapper.MapCar(car, bodyType, engine, transmission, wheelDrive, steering, isPreview);
            }).ToList();
        }

        void FillBodyTypes(IEnumerable<Car> cars, ModelGeneration modelGeneration, TimeFrame timeFrame)
        {
            timeFrame.BodyTypes = modelGeneration.BodyTypes.Where(bodyType => cars.Any(car => car.BodyTypeID == bodyType.ID))
                                                           .Select(bodyType => _bodyTypeMapper.MapBodyType(bodyType, false))
                                                           .ToList();
        }

        void FillEngines(IEnumerable<Car> cars, ModelGeneration modelGeneration, TimeFrame timeFrame)
        {
            timeFrame.Engines = modelGeneration.Engines.Where(engine => cars.Any(car => car.EngineID == engine.ID))
                                                       .Select(engine => _engineMapper.MapEngine(engine, false))
                                                       .ToList();
        }

        private void FillCarTechnicalSpecifications(Car car, SpecificationCategories categories, Units units, ContextData contextData)
        {
            var technicalSpecifications = car
                .Specifications
                .Where(specification => specification.Approved)
                .Select(specification=> 
                    _technicalSpecificationMapper.MapTechnicalSpecification(
                        specification,
                        categories.FindSpecification(specification.ID),
                        units[specification.Unit.ID]
                        )
                    )
                .Where(mappedSpecification=> mappedSpecification.RawValue.Length != 0)
                .ToList();

            contextData.CarTechnicalSpecifications.Add(car.ID, technicalSpecifications);
        }

        private void FillCarPacks(Car car, ContextData contextData, EquipmentGroups groups, EquipmentCategories categories, bool isPreview, string assetUrl)
        {
            var packs = car.Packs.Where(pack => pack.Availability != Availability.NotAvailable)
                                 .Select(pack => _packMapper.MapCarPack(pack, groups, categories, isPreview, assetUrl))
                                 .ToList();

            contextData.CarPacks.Add(car.ID, packs);
        }
        
        private void FillCarPackAccentColourCombination(ContextData contextData, Car car, ModelGeneration modelGeneration, bool isPreview, string assetUrl)
        {
            var packAccentColours = car.Packs.Where(pack => pack.Availability != Availability.NotAvailable)
                .ToDictionary(pack => pack.ID, pack => car.Generation.Packs[pack.ID].AccentColourCombinations.Where(accentColourCombination => accentColourCombination.IsAvailable).Select(acc => _colourMapper.MapPackAccentColourCombination(acc, car, modelGeneration, isPreview, assetUrl)).ToList());
            if (!contextData.CarPackAccentColourCombinations.ContainsKey(car.ID))
                contextData.CarPackAccentColourCombinations.Add(car.ID, new Dictionary<Guid, IList<AccentColourCombination>>());

            foreach (var entry in packAccentColours)
                contextData.CarPackAccentColourCombinations[car.ID].Add(entry.Key,new List<AccentColourCombination>(entry.Value));
        }

        private void FillCarRules(Car car, ContextData contextData, EquipmentGroups equipmentGroups, EquipmentItems equipmentItems)
        {
            contextData.CarRules.Add(car.ID, _ruleMapper.MapCarRules(car, equipmentGroups, equipmentItems));
        }

        private void FillCarParts(Car car, ContextData contextData)
        {
            contextData.CarParts.Add(car.ID,GetValidCarParts(car).Select(carPart => _carPartMapper.MapCarPart(carPart)).ToList());
        }

        private void FillCarColourCombinations(Car car, ContextData contextData, ExteriorColourTypes exteriorColourTypes, UpholsteryTypes upholsteryTypes, bool isPreview, string assetUrl)
        {
            var colourCombinations = car.ColourCombinations.Where(carColourCombination => carColourCombination.Approved)
                                      .Select(carColourCombination => _colourMapper.MapLinkedColourCombination(
                                          car.Generation, 
                                          carColourCombination,
                                          exteriorColourTypes[carColourCombination.ExteriorColour.Type.ID], 
                                          upholsteryTypes[carColourCombination.Upholstery.Type.ID],
                                          isPreview, 
                                          assetUrl))
                                      .OrderBy(carCombination => carCombination.ExteriorColour.Type.SortIndex)
                                      .ThenBy(carCombination => carCombination.ExteriorColour.SortIndex)
                                      .ThenBy(carCombination => carCombination.Upholstery.Type.SortIndex)
                                      .ThenBy(carCombination => carCombination.Upholstery.SortIndex)
                                      .ToList();

            var index = 0;
            foreach (var colourCombination in colourCombinations)
            {
                colourCombination.SortIndex = index++;
            }

            contextData.CarColourCombinations.Add(car.ID, colourCombinations);
        }

        private void FillCarEquipment(Car car, ContextData contextData, EquipmentCategories categories, EquipmentGroups groups, bool isPreview, String assetUrl)
        {
            contextData.CarEquipment.Add(car.ID,new CarEquipment
            {
                Accessories = car.Equipment.OfType<CarAccessory>()
                                           .Where(equipment => equipment.Availability != Availability.NotAvailable)
                                           .Select(accessory => _equipmentMapper.MapCarAccessory(accessory, groups.FindAccessory(accessory.ID), categories, isPreview, assetUrl))
                                           .ToList(),
                Options = car.Equipment.Where(equipment => equipment.Type == EquipmentType.Option && equipment.Availability != Availability.NotAvailable && ((CarOption)equipment).Visible)
                                       .Select(option => _equipmentMapper.MapCarOption((CarOption)option, groups.FindOption(option.ID), categories, isPreview, assetUrl))
                                       .ToList()
            });
        }

        private IEnumerable<ModelGenerationEquipmentItem> GetValidCarEquipment(Car car)
        {
            return car.Equipment.Select(eq => car.Generation.Equipment[eq.ID]);
        }

        private IEnumerable<IHasAssetSet> GetValidCarPacks(Car car)
        {
            return car.Packs.Select(pack => car.Generation.Packs[pack.ID]);
        }

        private static IEnumerable<ModelGenerationCarPart> GetValidCarParts(Car car)
        {
            return car.Generation.CarParts.Where(carPart => carPart.AssetSet.NumberOfAssets != 0);
        }

        void FillTransmissions(IEnumerable<Car> cars, ModelGeneration modelGeneration, TimeFrame timeFrame)
        {
            timeFrame.Transmissions = modelGeneration.Transmissions.Where(transmission => cars.Any(car => car.TransmissionID == transmission.ID))
                                                                   .Select(transmission => _transmissionMapper.MapTransmission(transmission,false))
                                                                   .ToList();

        }

        void FillWheelDrives(IEnumerable<Car> cars, ModelGeneration modelGeneration, TimeFrame timeFrame)
        {
            timeFrame.WheelDrives = modelGeneration.WheelDrives.Where(wheelDrive => cars.Any(car => car.WheelDriveID == wheelDrive.ID))
                                                               .Select(wheelDrive => _wheelDriveMapper.MapWheelDrive(wheelDrive, false))
                                                               .ToList();
        }

        void FillSteerings(IEnumerable<Car> cars, TimeFrame timeFrame)
        {
            timeFrame.Steerings = cars.Select(car => Steerings.GetSteerings()[car.SteeringID])
                                      .Distinct()
                                      .Select(_steeringMapper.MapSteering)
                                      .ToList();
        }

        private void FillColourCombinations(IEnumerable<Car> cars, ModelGeneration modelGeneration, TimeFrame timeFrame, Boolean isPreview, ExteriorColourTypes exteriorColourTypes, UpholsteryTypes upholsteryTypes, String assetUrl)
        {
            var colourCombinations = modelGeneration.ColourCombinations.Where(
                colourCombination => cars.Any(
                    car => car.ColourCombinations[colourCombination.ExteriorColour.ID, colourCombination.Upholstery.ID] != null && car.ColourCombinations[colourCombination.ExteriorColour.ID, colourCombination.Upholstery.ID].Approved))
                    .Select(colourCombination => 
                        _colourMapper.MapColourCombination(
                            modelGeneration, 
                            colourCombination, 
                            isPreview, 
                            exteriorColourTypes[colourCombination.ExteriorColour.Type.ID],
                            upholsteryTypes[colourCombination.Upholstery.Type.ID],
                            assetUrl))
                    .OrderBy(combination => combination.ExteriorColour.Type.SortIndex)
                    .ThenBy(combination => combination.ExteriorColour.SortIndex)
                    .ThenBy(combination => combination.Upholstery.Type.SortIndex)
                    .ThenBy(combination => combination.Upholstery.SortIndex)
                    .ToList();

            var index = 0;
            foreach (var colourCombination in colourCombinations)
            {
                colourCombination.SortIndex = index++;
            }

            timeFrame.ColourCombinations = colourCombinations;
        }

        private void FillSubModels(Model model, IList<ModelGenerationGrade> grades, IList<Car> cars, ModelGeneration modelGeneration, TimeFrame timeFrame, bool isPreview, EquipmentCategories categories, EquipmentGroups groups, ExteriorColourTypes exteriorColourTypes, String assetUrl)
        {
            var applicableSubModels = modelGeneration.SubModels.Where(submodel => cars.Any(car => car.SubModelID == submodel.ID)).ToList();

            var mappedSubModels = new List<SubModel>();

            var mappedSubModelGradeEquipments = new Dictionary<Guid, IReadOnlyDictionary<Guid, GradeEquipment>>();
            
            timeFrame.SubModelGrades = applicableSubModels.ToDictionary(
                subModel => subModel.ID,
                subModel => GetSubModelGrades(grades, subModel, timeFrame));

            foreach (var modelGenerationSubModel in applicableSubModels)
            {
                var subModelId = modelGenerationSubModel.ID;

                var mappedSubModel = _subModelMapper.MapSubModel(model, modelGenerationSubModel, timeFrame, isPreview);

                mappedSubModels.Add(mappedSubModel);

                mappedSubModelGradeEquipments.Add(subModelId, GetSubModelGradeEquipment(grades, modelGenerationSubModel, isPreview, categories, groups, exteriorColourTypes, assetUrl));

                AddSubModelToCar(cars, timeFrame, mappedSubModel);
            }
            
            timeFrame.SubModels = mappedSubModels;
            timeFrame.SubModelGradeEquipments = mappedSubModelGradeEquipments;
        }

        private IReadOnlyList<Grade> GetSubModelGrades(IEnumerable<ModelGenerationGrade> generationGrades, ModelGenerationSubModel modelGenerationSubModel, TimeFrame timeFrame)
        {
            return generationGrades
                    .Where(generationGrade => modelGenerationSubModel.Cars().Any(car => car.GradeID == generationGrade.ID))
                    .Select(grade => _gradeMapper.MapSubModelGrade(grade, modelGenerationSubModel, timeFrame.Cars))
                    .ToList();
        }

        private IReadOnlyDictionary<Guid, GradeEquipment> GetSubModelGradeEquipment(IEnumerable<ModelGenerationGrade> modelGenerationGrades, ModelGenerationSubModel modelGenerationSubModel, bool isPreview, EquipmentCategories categories, EquipmentGroups groups, ExteriorColourTypes exteriorColourTypes, String assetUrl)
        {
            return modelGenerationGrades.ToDictionary(
                modelGenerationGrade => modelGenerationGrade.ID,
                modelGenerationGrade => GetGradeEquipment(modelGenerationGrade, modelGenerationGrade.Cars().Filter(isPreview)
                                                                                                                   .Where(car => car.SubModelID == modelGenerationSubModel.ID)
                                                                                                                   .ToList(), isPreview, categories, groups, exteriorColourTypes, assetUrl));
        }

        private static void AddSubModelToCar(IEnumerable<Car> cars, TimeFrame timeFrame, SubModel mappedSubModel)
        {
            var applicableCars = cars
                    .Where(car => car.SubModelID == mappedSubModel.ID)
                    .Select(car => timeFrame.Cars.Single(contextCar => contextCar.ID == car.ID));

            foreach (var applicableCar in applicableCars)
                applicableCar.SubModel = mappedSubModel;
        }

        void FillGrades(IList<Car> cars, ModelGeneration modelGeneration, TimeFrame timeFrame)
        {
            var applicableGrades = modelGeneration.Grades.Where(grade => cars.Any(car => car.GradeID == grade.ID)).ToArray();

            var mappedGrades = new List<Grade>();

            foreach (var grade in applicableGrades)
            {
                var mappedGrade = _gradeMapper.MapGenerationGrade(grade, timeFrame.Cars);

                if (mappedGrade.BasedUpon != null && applicableGrades.All(grd => grd.ID != mappedGrade.BasedUpon.ID))
                    mappedGrade.BasedUpon = null;

                mappedGrades.Add(mappedGrade);


                AddGradeToCar(cars, timeFrame, mappedGrade);
            }

            timeFrame.Grades = mappedGrades;
        }

        private void AddGradeToCar(IEnumerable<Car> cars, TimeFrame timeFrame, Grade mappedGrade)
        {
            var gradeID = mappedGrade.ID;
            var applicableCars = cars.Where(car => car.GradeID == gradeID)
                .Select(car => timeFrame.Cars.Single(contextCar => contextCar.ID == car.ID));

            foreach (var applicableCar in applicableCars)
                applicableCar.Grade = _carMapper.CopyGrade(mappedGrade);
        }

        void FillGradeEquipment(EquipmentCategories categories, EquipmentGroups groups, IEnumerable<Car> cars, IEnumerable<ModelGenerationGrade> grades, TimeFrame timeFrame, bool isPreview, ExteriorColourTypes exteriorColourTypes, String assetUrl)
        {
            timeFrame.GradeEquipments = grades.ToDictionary(
                grade => grade.ID,
                grade => GetGradeEquipment(grade, grade.Cars().Filter(isPreview).Intersect(cars).ToList(), isPreview, categories, groups, exteriorColourTypes, assetUrl));
        }

        private GradeEquipment GetGradeEquipment(ModelGenerationGrade grade, IReadOnlyList<Car> gradeCars, bool isPreview, EquipmentCategories categories, EquipmentGroups groups, ExteriorColourTypes exteriorColourTypes, String assetUrl)
        {
            var accessories = grade.Equipment.OfType<ModelGenerationGradeAccessory>()
                    .Where(accessory => gradeCars.Any(car => car.Equipment[accessory.ID] != null && car.Equipment[accessory.ID].Availability != Availability.NotAvailable))
                    .Select(accessory => _equipmentMapper.MapGradeAccessory(accessory, groups.FindAccessory(accessory.ID), categories, gradeCars, isPreview, exteriorColourTypes, assetUrl))
                    .ToList();

            var options = grade.Equipment.OfType<ModelGenerationGradeOption>()
                     .Where(option => gradeCars.Any(car => car.Equipment[option.ID] != null && car.Equipment[option.ID].Availability != Availability.NotAvailable && option.Visible))
                     .Select(option => _equipmentMapper.MapGradeOption(option, groups.FindOption(option.ID), categories, gradeCars, isPreview, exteriorColourTypes, assetUrl))
                     .ToList();

            return new GradeEquipment { Accessories = accessories, Options = options };
        }

        private void FillGradePacks(IEnumerable<ModelGenerationGrade> grades, TimeFrame timeFrame, bool isPreview)
        {

            timeFrame.GradePacks = grades.ToDictionary(
                grade => grade.ID,
                grade =>
                {
                    var gradeCars = grade.Cars().Filter(isPreview).ToList();
                    var gradePacks = grade.Packs;
                    var generationPacks = grade.Generation.Packs;

                    return gradePacks
                        .Select(gradePack => _packMapper.MapGradePack(gradePack, generationPacks[gradePack.ID], gradeCars))
                        .Where(gradePack => gradePack.OptionalOn.Any() || gradePack.StandardOn.Any()) // do not publish packs that are not available on any car
                        .ToReadOnlyList();
                });
        }

        private void FillSubModelGradePacks(IEnumerable<ModelGenerationGrade> grades, ModelGeneration modelGeneration, TimeFrame timeFrame, Boolean isPreview)
        {
            timeFrame.SubModelGradePacks = timeFrame.SubModels.ToDictionary(
                subModel => subModel.ID,
                subModel => GetSubModelGradePacks(grades, modelGeneration.SubModels[subModel.ID], isPreview));
        }

        private IReadOnlyDictionary<Guid, IReadOnlyList<GradePack>> GetSubModelGradePacks(IEnumerable<ModelGenerationGrade> grades, ModelGenerationSubModel subModel, Boolean isPreview)
        {
            return grades.ToDictionary(
                grade => grade.ID,
                grade => GetSubModelGradePacks(grade, subModel, isPreview));
        }

        private IReadOnlyList<GradePack> GetSubModelGradePacks(ModelGenerationGrade grade, ModelGenerationSubModel subModel, Boolean isPreview)
        {
            var subModelGradeCars = grade.Cars().Filter(isPreview).Where(car => car.SubModelID == subModel.ID).ToList();
            var gradePacks = grade.Packs;
            var generationPacks = grade.Generation.Packs;

            var mappedGradePacks = gradePacks
                                    .Select(gradePack => _packMapper.MapGradePack(gradePack, generationPacks[gradePack.ID], subModelGradeCars))
                                    .Where(gradePack => gradePack.OptionalOn.Any() || gradePack.StandardOn.Any()); // do not publish packs that are not available on any car

            return mappedGradePacks.ToList();
        }

        private void FillEquipmentCategories(IList<EquipmentCategory> categories, ContextData contextData)
        {
            contextData.EquipmentCategories = categories
                .Flatten(category => category.Categories)
                .Select(_categoryMapper.MapEquipmentCategory)
                .ToList();
        }

        private void FillSpecificationCategories(IList<SpecificationCategory> categories, ContextData contextData)
        {
            contextData.SpecificationCategories = categories
                .Flatten(category => category.Categories)
                .Select(_categoryMapper.MapSpecificationCategory)
                .ToList();
        }
    }
}
