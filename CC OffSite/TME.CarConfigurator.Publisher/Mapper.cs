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
            ICategoryMapper categoryMapper)
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
                progress.Report(new PublishProgress("Get generation grades"));
                var grades = modelGeneration.Grades.Where(grade => cars.Any(car => car.GradeID == grade.ID)).ToArray();

                progress.Report(new PublishProgress("Fill generation assets"));
                FillAssets(modelGeneration, contextData);
                progress.Report(new PublishProgress("Fill generation body types"));
                FillBodyTypes(cars, modelGeneration, contextData);
                progress.Report(new PublishProgress("Fill generation engines"));
                FillEngines(cars, modelGeneration, contextData);
                progress.Report(new PublishProgress("Fill generation transmissions"));
                FillTransmissions(cars, modelGeneration, contextData);
                progress.Report(new PublishProgress("Fill generation wheeldrives"));
                FillWheelDrives(cars, modelGeneration, contextData);
                progress.Report(new PublishProgress("Fill generation steerings"));
                FillSteerings(cars, contextData);
                progress.Report(new PublishProgress("Fill generation colour combinations"));
                FillColourCombinations(cars, modelGeneration, contextData, isPreview);
                progress.Report(new PublishProgress("Fill generation cars"));
                FillCars(cars, contextData);
                progress.Report(new PublishProgress("Fill generation grades"));
                FillGrades(cars, modelGeneration, contextData);
                progress.Report(new PublishProgress("Fill generation car assets"));
                FillCarAssets(cars, contextData, modelGeneration);
                progress.Report(new PublishProgress("Fill grade equipment"));
                FillGradeEquipment(grades, modelGeneration, contextData, isPreview);
                progress.Report(new PublishProgress("Fill generation submodels"));
                FillSubModels(grades, cars, modelGeneration, contextData, isPreview);
                progress.Report(new PublishProgress("Fill generation grade packs"));
                FillGradePacks(grades, contextData, isPreview);
                progress.Report(new PublishProgress("Fill generation equipment categories"));
                FillEquipmentCategories(contextData);
                progress.Report(new PublishProgress("Fill submodel assets"));
                FillSubModelAssets(grades, modelGeneration, contextData);
                progress.Report(new PublishProgress("Fill submodel grade packs"));
                FillSubModelGradePacks(grades, modelGeneration, contextData, isPreview);
                progress.Report(new PublishProgress("Fill generation specification categories"));
                FillSpecificationCategories(contextData);

                progress.Report(new PublishProgress("Fill publication timeframes"));
                context.TimeFrames[language] = _timeFrameMapper.GetTimeFrames(language, context);
            }
        }

        private static IEnumerable<KeyValuePair<string, Tuple<ModelGeneration, Model>>> GetModelGenerationForEachLanguage(String brand, String countryCode, Guid generationID)
        {
            var country = MyContext.GetContext().Countries.Single(ctry => ctry.Code == countryCode);
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
                FillCarAssets(car, contextData, modelGeneration, car.Generation.BodyTypes[car.BodyTypeID]);
                FillCarAssets(car, contextData, modelGeneration, car.Generation.Engines[car.EngineID]);
                FillCarAssets(car, contextData, modelGeneration, car.Generation.Grades[car.GradeID]);
                FillCarAssets(car, contextData, modelGeneration, car.Generation.Transmissions[car.TransmissionID]);
            }
        }

        private void FillCarAssets(Car car, ContextData contextData, ModelGeneration modelGeneration, IHasAssetSet objectWithAssetSet)
        {
            var objectAssetsOnCarLevel = GetObjectAssetsOnCarLevel(car, modelGeneration, objectWithAssetSet);
            var objectAssetsOnGenerationLevel = GetObjectAssetsOnGenerationLevel(objectWithAssetSet.GetObjectID(), contextData.Assets);

            var allCarAssetsForThisObject = objectAssetsOnCarLevel.Concat(objectAssetsOnGenerationLevel).ToList();

            contextData.CarAssets[car.ID].Add(objectWithAssetSet.GetObjectID(), allCarAssetsForThisObject);
        }

        private void FillSubModelAssets(IEnumerable<ModelGenerationGrade> grades, ModelGeneration modelGeneration, ContextData contextData)
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

            return objectAssetsOnCarLevel.Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList();
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
                                    colour => colour.AssetSet.Assets.GetGenerationAssets()
                                        .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private IEnumerable<KeyValuePair<Guid, List<Asset>>> GetUpholsteryAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.ColourCombinations
                                  .Upholsteries()
                                  .ToDictionary(
                                    upholstery => upholstery.ID,
                                    upholstery => upholstery.AssetSet.Assets.GetGenerationAssets()
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

        void FillCars(IEnumerable<Car> cars, ContextData contextData)
        {
            foreach (var car in cars)
            {
                var bodyType = contextData.BodyTypes.Single(type => type.ID == car.BodyTypeID);
                var engine = contextData.Engines.Single(eng => eng.ID == car.EngineID);
                var transmission = contextData.Transmissions.Single(trans => trans.ID == car.TransmissionID);
                var wheelDrive = contextData.WheelDrives.Single(drive => drive.ID == car.WheelDriveID);
                var steering = contextData.Steerings.Single(steer => steer.ID == car.SteeringID);
                contextData.CarAssets.Add(car.ID, new Dictionary<Guid, IList<Asset>>());
                contextData.Cars.Add(_carMapper.MapCar(car, bodyType, engine, transmission, wheelDrive, steering));
            }
        }

        void FillBodyTypes(IEnumerable<Car> cars, ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var bodyType in modelGeneration.BodyTypes.Where(bodyType => cars.Any(car => car.BodyTypeID == bodyType.ID)))
                contextData.BodyTypes.Add(_bodyTypeMapper.MapBodyType(bodyType));
        }

        void FillEngines(IEnumerable<Car> cars, ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var engine in modelGeneration.Engines.Where(engine => cars.Any(car => car.EngineID == engine.ID)))
                contextData.Engines.Add(_engineMapper.MapEngine(engine));
        }

        void FillTransmissions(IEnumerable<Car> cars, ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var transmission in modelGeneration.Transmissions.Where(transmission => cars.Any(car => car.TransmissionID == transmission.ID)))
                contextData.Transmissions.Add(_transmissionMapper.MapTransmission(transmission));
        }

        private void FillColourCombinations(IEnumerable<Car> cars, ModelGeneration modelGeneration, ContextData contextData, Boolean isPreview)
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
                contextData.ColourCombinations.Add(colourCombination);
            }
        }

        void FillWheelDrives(IEnumerable<Car> cars, ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var wheelDrive in modelGeneration.WheelDrives.Where(wheelDrive => cars.Any(car => car.WheelDriveID == wheelDrive.ID)))
                contextData.WheelDrives.Add(_wheelDriveMapper.MapWheelDrive(wheelDrive));
        }

        private void FillSubModels(ModelGenerationGrade[] grades, IList<Car> cars, ModelGeneration modelGeneration, ContextData contextData, bool isPreview)
        {
            var applicableSubModels = modelGeneration.SubModels.Where(submodel => cars.Any(car => car.SubModelID == submodel.ID));

            foreach (var modelGenerationSubModel in applicableSubModels)
            {
                var subModelId = modelGenerationSubModel.ID;

                FillSubModelGrades(grades, modelGenerationSubModel, contextData);

                var mappedSubModel = _subModelMapper.MapSubModel(modelGenerationSubModel, contextData, isPreview);

                contextData.SubModels.Add(mappedSubModel);

                FillSubModelGradeEquipment(grades, modelGenerationSubModel, contextData, isPreview);

                AddSubModelToCar(cars, contextData, subModelId, mappedSubModel);
            }
        }

        private void FillSubModelGrades(IEnumerable<ModelGenerationGrade> generationGrades, ModelGenerationSubModel modelGenerationSubModel, ContextData contextData)
        {
            contextData.SubModelGrades.Add(modelGenerationSubModel.ID, generationGrades
                .Where(generationGrade => modelGenerationSubModel.Cars()
                                                              .Any(car => car.GradeID == generationGrade.ID))
                .Select(grade => _gradeMapper.MapSubModelGrade(grade, modelGenerationSubModel, contextData.Cars)).ToList());
        }

        private static void AddSubModelToCar(IEnumerable<Car> cars, ContextData contextData, Guid subModelId, SubModel mappedSubModel)
        {
            var applicableCars = cars
                    .Where(car => car.SubModelID == subModelId)
                    .Select(car => contextData.Cars.Single(contextCar => contextCar.ID == car.ID));

            foreach (var applicableCar in applicableCars)
                applicableCar.SubModel = mappedSubModel;
        }

        void FillGrades(IList<Car> cars, ModelGeneration modelGeneration, ContextData contextData)
        {
            var applicableGrades = modelGeneration.Grades.Where(grade => cars.Any(car => car.GradeID == grade.ID)).ToArray();

            foreach (var grade in applicableGrades)
            {
                var mappedGrade = _gradeMapper.MapGenerationGrade(grade, contextData.Cars);

                contextData.Grades.Add(mappedGrade);

                AddGradeToCar(cars, contextData, grade, mappedGrade);
            }
        }

        private static void AddGradeToCar(IEnumerable<Car> cars, ContextData contextData, ModelGenerationGrade grade, Grade mappedGrade)
        {
            var gradeID = grade.ID;
            var applicableCars = cars.Where(car => car.GradeID == gradeID)
                .Select(car => contextData.Cars.Single(contextCar => contextCar.ID == car.ID));

            foreach (var applicableCar in applicableCars)
                applicableCar.Grade = mappedGrade;
        }

        void FillSteerings(IEnumerable<Car> cars, ContextData contextData)
        {
            var steerings = cars.Select(car => Steerings.GetSteerings()[car.SteeringID]).Distinct();

            foreach (var steering in steerings)
                contextData.Steerings.Add(_steeringMapper.MapSteering(steering));
        }

        private void FillSubModelGradeEquipment(IEnumerable<ModelGenerationGrade> modelGenerationGrades, ModelGenerationSubModel modelGenerationSubModel, ContextData contextData, bool isPreview)
        {
            contextData.SubModelGradeEquipment.Add(modelGenerationSubModel.ID,
                                                   modelGenerationGrades.ToDictionary(modelGenerationGrade =>
                                                                                            modelGenerationGrade.ID,
                                                                                      modelGenerationGrade =>
                                                                                            GetGradeEquipment(modelGenerationGrade, modelGenerationGrade.Cars().Filter(isPreview)
                                                                                                                                            .Where(car => car.SubModelID == modelGenerationSubModel.ID)
                                                                                                                                            .ToList(),
                                                                                                              modelGenerationSubModel.Generation,
                                                                                                              isPreview)));
        }

        void FillGradeEquipment(IEnumerable<ModelGenerationGrade> grades, ModelGeneration modelGeneration, ContextData data, Boolean isPreview)
        {
            foreach (var grade in grades)
            {
                var gradeCars = grade.Cars().Filter(isPreview).ToList();

                var gradeEquipment = GetGradeEquipment(grade, gradeCars, modelGeneration, isPreview);

                data.GradeEquipment.Add(grade.ID, gradeEquipment);
            }
        }

        private GradeEquipment GetGradeEquipment(ModelGenerationGrade grade, List<Car> gradeCars, ModelGeneration modelGeneration, bool isPreview)
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

        private void FillGradePacks(IEnumerable<ModelGenerationGrade> grades, ContextData contextData, bool isPreview)
        {
            foreach (var grade in grades)
            {
                var gradeCars = grade.Cars().Filter(isPreview).ToList();
                var gradePacks = grade.Packs;
                var generationPacks = grade.Generation.Packs;

                var mappedGradePacks = gradePacks
                    .Select(gradePack => _packMapper.MapGradePack(gradePack, generationPacks[gradePack.ID], gradeCars))
                    .Where(gradePack => gradePack.OptionalOn.Any() || gradePack.StandardOn.Any()) // do not publish packs that are not available on any car
                    .ToList();

                contextData.GradePacks.Add(grade.ID, mappedGradePacks);
            }
        }

        private void FillSubModelGradePacks(IEnumerable<ModelGenerationGrade> grades, ModelGeneration modelGeneration, ContextData contextData, Boolean isPreview)
        {
            foreach (var subModel in modelGeneration.SubModels)
            {
                FillSubModelGradePacks(grades, subModel, contextData, isPreview);
            }
        }

        private void FillSubModelGradePacks(IEnumerable<ModelGenerationGrade> grades, ModelGenerationSubModel subModel, ContextData contextData, Boolean isPreview)
        {
            var gradePacks = grades.ToDictionary(
                grade => grade.ID,
                grade => GetSubModelGradePacks(grade, subModel, contextData, isPreview));

            contextData.SubModelGradePacks.Add(subModel.ID, gradePacks);
        }

        private IReadOnlyList<Repository.Objects.Packs.GradePack> GetSubModelGradePacks(ModelGenerationGrade grade, ModelGenerationSubModel subModel, ContextData contextData, Boolean isPreview)
        {
            var subModelGradeCars = grade.Cars().Filter(isPreview).Where(car => car.SubModelID == subModel.ID).ToList();
            var gradePacks = grade.Packs;
            var generationPacks = grade.Generation.Packs;

            var mappedGradePacks = gradePacks
                                    .Select(gradePack => _packMapper.MapGradePack(gradePack, generationPacks[gradePack.ID], subModelGradeCars))
                                    .Where(gradePack => gradePack.OptionalOn.Any() || gradePack.StandardOn.Any()); // do not publish packs that are not available on any car

            return mappedGradePacks.ToList();
        }

        private void FillEquipmentCategories(ContextData contextData)
        {
            var mappedCategories = EquipmentCategories.GetEquipmentCategories()
                .Flatten(category => category.Categories)
                .Select(_categoryMapper.MapEquipmentCategory);

            foreach (var mappedCategory in mappedCategories)
                contextData.EquipmentCategories.Add(mappedCategory);
        }

        private void FillSpecificationCategories(ContextData contextData)
        {
            var mappedCategories = Administration.SpecificationCategories.GetSpecificationCategories()
                                                                         .Flatten(category => category.Categories)
                                                                         .Select(_categoryMapper.MapSpecificationCategory)
                                                                         .ToList();

            foreach (var mappedCategory in mappedCategories)
                contextData.SpecificationCategories.Add(mappedCategory);
        }
    }
}
