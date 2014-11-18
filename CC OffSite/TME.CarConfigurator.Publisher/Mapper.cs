using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.Administration;
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
using Asset = TME.CarConfigurator.Repository.Objects.Assets.Asset;
using GradePack = TME.CarConfigurator.Repository.Objects.Packs.GradePack;
using Car = TME.CarConfigurator.Administration.Car;
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
            ICarPartMapper carPartMapper)
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

                context.ModelGenerations[language] = modelGeneration;
                context.ContextData[language] = contextData;

                // fill contextData
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
                    FillCars(timeFrameCars, timeFrame);
                    
                    progress.Report(new PublishProgress("Fill generation grades"));
                    FillGrades(timeFrameCars, modelGeneration, timeFrame);
                    progress.Report(new PublishProgress("Fill generation grade packs"));
                    FillGradePacks(timeFrameGrades, timeFrame, isPreview);
                    progress.Report(new PublishProgress("Fill grade equipment"));
                    FillGradeEquipment(timeFrameCars, timeFrameGrades, timeFrame, isPreview);
                    progress.Report(new PublishProgress("Fill generation submodels"));
                    FillSubModels(timeFrameGrades, timeFrameCars, modelGeneration, timeFrame, isPreview);
                    progress.Report(new PublishProgress("Fill submodel grade packs"));
                    FillSubModelGradePacks(timeFrameGrades, modelGeneration, timeFrame, isPreview);
                    progress.Report(new PublishProgress("Fill generation equipment categories"));
                    FillEquipmentCategories(timeFrame);
                    progress.Report(new PublishProgress("Fill generation specification categories"));
                    FillSpecificationCategories(timeFrame);
                    progress.Report(new PublishProgress("Fill generation colour combinations"));
                    FillColourCombinations(timeFrameCars, modelGeneration, timeFrame, isPreview);
                }

                progress.Report(new PublishProgress("Fill submodel assets"));
                FillSubModelAssets(grades, modelGeneration, contextData);
                progress.Report(new PublishProgress("Fill generation car assets"));
                FillCarAssets(cars, contextData, modelGeneration);
                progress.Report(new PublishProgress("Fill car parts"));
                FillCarParts(cars, modelGeneration, contextData);
                progress.Report(new PublishProgress("Fill car packs"));
                FillCarPacks(cars, contextData);
            }
        }

        private void FillCarParts(IEnumerable<Car> cars, ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var car in cars)
            {
                var carPartsWithAssets = modelGeneration.CarParts.Where(carPart => carPart.AssetSet.NumberOfAssets != 0).ToList();

                contextData.CarCarParts.Add(car.ID, carPartsWithAssets.Select(carPart => _carPartMapper.MapCarPart(carPart)).ToList());

                FillCarPartAssets(modelGeneration, contextData, carPartsWithAssets, car);
            }
        }

        private void FillCarPartAssets(ModelGeneration modelGeneration, ContextData contextData, IEnumerable<ModelGenerationCarPart> carParts, Car car)
        {
            foreach (var carPart in carParts)
            {
                var assets = carPart.AssetSet.Assets.Where(assetSetAsset => assetSetAsset.BodyType.ID == car.BodyTypeID && assetSetAsset.Steering.ID == car.SteeringID);
                contextData.CarAssets[car.ID].Add(carPart.ID, new List<Asset>(assets.Select(asset => _assetMapper.MapCarAssetSetAsset(asset, modelGeneration)).ToList()));
            }
        }

        private void FillCarPacks(IEnumerable<Car> cars, ContextData contextData)
        {
            foreach (var car in cars)
            {
                var packs = car.Packs.Where(pack => pack.Availability != Administration.Enums.Availability.NotAvailable).Select(_packMapper.MapCarPack).ToList();
                contextData.CarPacks.Add(car.ID, packs);
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
                foreach (var pack in car.Packs)
                    FillCarAssets(car, contextData, modelGeneration, car.Generation.Packs[pack.ID]);
            }
        }

        private void FillCarAssets(Car car, ContextData contextData, ModelGeneration modelGeneration, IHasAssetSet objectWithAssetSet)
        {
            if (objectWithAssetSet == null) return;
            var objectAssetsOnCarLevel = GetObjectAssetsOnCarLevel(car, modelGeneration, objectWithAssetSet);

            var hasMappedGenerationAssets = contextData.Assets.ContainsKey(objectWithAssetSet.GetObjectID());

            var objectAssetsOnGenerationLevel = hasMappedGenerationAssets ?
                GetObjectAssetsOnGenerationLevel(objectWithAssetSet.GetObjectID(), contextData.Assets) :
                objectWithAssetSet.AssetSet.Assets.GetGenerationAssets().Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList();

            var allCarAssetsForThisObject = objectAssetsOnCarLevel.Concat(objectAssetsOnGenerationLevel).ToList();

            contextData.CarAssets[car.ID].Add(objectWithAssetSet.GetObjectID(), allCarAssetsForThisObject);
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

        private IEnumerable<Asset> GetObjectAssetsOnGenerationLevel(Guid objectId, IDictionary<Guid, IList<Asset>> assets)
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

        void FillCars(IEnumerable<Car> cars, TimeFrame timeFrame)
        {
            timeFrame.Cars = cars.Select(car => {
                var bodyType = timeFrame.BodyTypes.Single(type => type.ID == car.BodyTypeID);
                var engine = timeFrame.Engines.Single(eng => eng.ID == car.EngineID);
                var transmission = timeFrame.Transmissions.Single(trans => trans.ID == car.TransmissionID);
                var wheelDrive = timeFrame.WheelDrives.Single(drive => drive.ID == car.WheelDriveID);
                var steering = timeFrame.Steerings.Single(steer => steer.ID == car.SteeringID);
                //contextData.CarAssets.Add(car.ID, new Dictionary<Guid, IList<Asset>>());
                return _carMapper.MapCar(car, bodyType, engine, transmission, wheelDrive, steering);
            }).ToList();
        }

        void FillBodyTypes(IEnumerable<Car> cars, ModelGeneration modelGeneration, TimeFrame timeFrame)
        {
            timeFrame.BodyTypes = modelGeneration.BodyTypes.Where(bodyType => cars.Any(car => car.BodyTypeID == bodyType.ID))
                                                           .Select(_bodyTypeMapper.MapBodyType)
                                                           .ToList();
        }

        void FillEngines(IEnumerable<Car> cars, ModelGeneration modelGeneration, TimeFrame timeFrame)
        {
            timeFrame.Engines = modelGeneration.Engines.Where(engine => cars.Any(car => car.EngineID == engine.ID))
                                                       .Select(_engineMapper.MapEngine)
                                                       .ToList();
        }

        void FillTransmissions(IEnumerable<Car> cars, ModelGeneration modelGeneration, TimeFrame timeFrame)
        {
            timeFrame.Transmissions = modelGeneration.Transmissions.Where(transmission => cars.Any(car => car.TransmissionID == transmission.ID))
                                                                   .Select(_transmissionMapper.MapTransmission)
                                                                   .ToList();

        }

        void FillWheelDrives(IEnumerable<Car> cars, ModelGeneration modelGeneration, TimeFrame timeFrame)
        {
            timeFrame.WheelDrives = modelGeneration.WheelDrives.Where(wheelDrive => cars.Any(car => car.WheelDriveID == wheelDrive.ID))
                                                               .Select(_wheelDriveMapper.MapWheelDrive)
                                                               .ToList();
        }

        void FillSteerings(IEnumerable<Car> cars, TimeFrame timeFrame)
        {
            timeFrame.Steerings = cars.Select(car => Steerings.GetSteerings()[car.SteeringID])
                                      .Distinct()
                                      .Select(_steeringMapper.MapSteering)
                                      .ToList();
        }

        private void FillColourCombinations(IEnumerable<Car> cars, ModelGeneration modelGeneration, TimeFrame timeFrame, Boolean isPreview)
        {
            var colourCombinations = modelGeneration.ColourCombinations.Where(
                colourCombination => cars.Any(
                    car => car.ColourCombinations[colourCombination.ExteriorColour.ID, colourCombination.Upholstery.ID] != null && car.ColourCombinations[colourCombination.ExteriorColour.ID, colourCombination.Upholstery.ID].Approved))
                    .Select(colourCombination => _colourMapper.MapColourCombination(modelGeneration, colourCombination, isPreview))
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

        private void FillSubModels(IList<ModelGenerationGrade> grades, IList<Car> cars, ModelGeneration modelGeneration, TimeFrame timeFrame, bool isPreview)
        {
            var applicableSubModels = modelGeneration.SubModels.Where(submodel => cars.Any(car => car.SubModelID == submodel.ID));

            var mappedSubModels = new List<SubModel>();

            var mappedSubModelGradeEquipments = new Dictionary<Guid, IReadOnlyDictionary<Guid, GradeEquipment>>();
            
            timeFrame.SubModelGrades = applicableSubModels.ToDictionary(
                subModel => subModel.ID,
                subModel => GetSubModelGrades(grades, subModel, timeFrame));

            foreach (var modelGenerationSubModel in applicableSubModels)
            {
                var subModelId = modelGenerationSubModel.ID;

                var mappedSubModel = _subModelMapper.MapSubModel(modelGenerationSubModel, timeFrame, isPreview);

                mappedSubModels.Add(mappedSubModel);

                mappedSubModelGradeEquipments.Add(subModelId, GetSubModelGradeEquipment(grades, modelGenerationSubModel, timeFrame, isPreview));

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

        private IReadOnlyDictionary<Guid, GradeEquipment> GetSubModelGradeEquipment(IEnumerable<ModelGenerationGrade> modelGenerationGrades, ModelGenerationSubModel modelGenerationSubModel, TimeFrame timeFrame, bool isPreview)
        {
            return modelGenerationGrades.ToDictionary(
                modelGenerationGrade => modelGenerationGrade.ID,
                modelGenerationGrade => GetSubModelGradeEquipment(modelGenerationGrade, modelGenerationGrade.Cars().Filter(isPreview)
                                                                                                                   .Where(car => car.SubModelID == modelGenerationSubModel.ID)
                                                                                                                   .ToList(), modelGenerationSubModel, timeFrame));
        }

        private static GradeEquipment GetSubModelGradeEquipment(ModelGenerationGrade modelGenerationGrade, IEnumerable<Car> cars, ModelGenerationSubModel modelGenerationSubModel, TimeFrame timeFrame)
        {
            var accessories =
                timeFrame.GradeEquipments[modelGenerationGrade.ID].Accessories.Where(
                    accessory =>
                        cars.Any(
                            car =>
                                car.Equipment[accessory.ID] != null &&
                                car.Equipment[accessory.ID].Availability !=
                                Administration.Enums.Availability.NotAvailable));

            var options = timeFrame.GradeEquipments[modelGenerationGrade.ID].Options.Where(
                option => cars.Any(
                            car =>
                                car.Equipment[option.ID] != null &&
                                car.Equipment[option.ID].Availability !=
                                Administration.Enums.Availability.NotAvailable));

            foreach (var gradeAccessory in accessories)
            {
                gradeAccessory.KeyFeature = modelGenerationSubModel.Equipment[gradeAccessory.ID].KeyFeature;
            }

            foreach (var gradeOption in options)
            {
                gradeOption.KeyFeature = modelGenerationSubModel.Equipment[gradeOption.ID].KeyFeature;
            }

            return new GradeEquipment { Accessories = accessories.ToList(), Options = options.ToList() };
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

            var mappedGrades = new List<Repository.Objects.Grade>();

            foreach (var grade in applicableGrades)
            {
                var mappedGrade = _gradeMapper.MapGenerationGrade(grade, timeFrame.Cars);

                if (mappedGrade.BasedUpon != null && !applicableGrades.Any(grd => grd.ID == mappedGrade.BasedUpon.ID))
                    mappedGrade.BasedUpon = null;

                mappedGrades.Add(mappedGrade);


                AddGradeToCar(cars, timeFrame, mappedGrade);
            }

            timeFrame.Grades = mappedGrades;
        }

        private static void AddGradeToCar(IEnumerable<Car> cars, TimeFrame timeFrame, Grade mappedGrade)
        {
            var gradeID = mappedGrade.ID;
            var applicableCars = cars.Where(car => car.GradeID == gradeID)
                .Select(car => timeFrame.Cars.Single(contextCar => contextCar.ID == car.ID));

            foreach (var applicableCar in applicableCars)
                applicableCar.Grade = mappedGrade;
        }




        void FillGradeEquipment(IEnumerable<Car> cars, IEnumerable<ModelGenerationGrade> grades, TimeFrame timeFrame, Boolean isPreview)
        {
            timeFrame.GradeEquipments = grades.ToDictionary(
                grade => grade.ID,
                grade => GetGradeEquipment(grade, grade.Cars().Filter(isPreview).Intersect(cars).ToList(), isPreview));
        }

        private GradeEquipment GetGradeEquipment(ModelGenerationGrade grade, IReadOnlyList<Car> gradeCars, bool isPreview)
        {
            var crossModelEquipment = EquipmentItems.GetEquipmentItems();
            var categories = EquipmentCategories.GetEquipmentCategories();

            var accessories = grade.Equipment.OfType<ModelGenerationGradeAccessory>()
                    .Where(accessory => gradeCars.Any(car => car.Equipment[accessory.ID] != null && car.Equipment[accessory.ID].Availability != Administration.Enums.Availability.NotAvailable))
                    .Select(accessory => _equipmentMapper.MapGradeAccessory(accessory, crossModelEquipment[accessory.ID], categories, gradeCars, isPreview))
                    .ToList();

            var options = grade.Equipment.OfType<ModelGenerationGradeOption>()
                     .Where(option => gradeCars.Any(car => car.Equipment[option.ID] != null && car.Equipment[option.ID].Availability != Administration.Enums.Availability.NotAvailable && option.Visible))
                     .Select(option => _equipmentMapper.MapGradeOption(option, crossModelEquipment[option.ID], categories, gradeCars, isPreview))
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

        private void FillSubModelGradePacks(IList<ModelGenerationGrade> grades, ModelGeneration modelGeneration, TimeFrame timeFrame, Boolean isPreview)
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

        private IReadOnlyList<Repository.Objects.Packs.GradePack> GetSubModelGradePacks(ModelGenerationGrade grade, ModelGenerationSubModel subModel, Boolean isPreview)
        {
            var subModelGradeCars = grade.Cars().Filter(isPreview).Where(car => car.SubModelID == subModel.ID).ToList();
            var gradePacks = grade.Packs;
            var generationPacks = grade.Generation.Packs;

            var mappedGradePacks = gradePacks
                                    .Select(gradePack => _packMapper.MapGradePack(gradePack, generationPacks[gradePack.ID], subModelGradeCars))
                                    .Where(gradePack => gradePack.OptionalOn.Any() || gradePack.StandardOn.Any()); // do not publish packs that are not available on any car

            return mappedGradePacks.ToList();
        }

        private void FillEquipmentCategories(TimeFrame timeFrame)
        {
            timeFrame.EquipmentCategories = EquipmentCategories.GetEquipmentCategories()
                .Flatten(category => category.Categories)
                .Select(_categoryMapper.MapEquipmentCategory)
                .ToList();
        }

        private void FillSpecificationCategories(TimeFrame timeFrame)
        {
            timeFrame.SpecificationCategories = SpecificationCategories.GetSpecificationCategories()
                .Flatten(category => category.Categories)
                .Select(_categoryMapper.MapSpecificationCategory)
                .ToList();
        }
    }
}
