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
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;
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
            IColourMapper colourMapper)
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
        }

        public Task MapAsync(IContext context)
        {
            return Task.Run(() => Map(context));
        }

        private void Map(IContext context)
        {
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
                var generation = _generationMapper.MapGeneration(model, modelGeneration, isPreview);
                contextData.Generations.Add(generation);
                contextData.Models.Add(_modelMapper.MapModel(model));

                var cars = modelGeneration.Cars.Where(car => isPreview ? car.Preview : car.Approved).ToList();
                var grades = modelGeneration.Grades.Where(grade => cars.Any(car => car.GradeID == grade.ID)).ToArray();

                FillAssets(modelGeneration, contextData);
                FillBodyTypes(cars, modelGeneration, contextData);
                FillEngines(cars, modelGeneration, contextData);
                FillTransmissions(cars, modelGeneration, contextData);
                FillWheelDrives(cars, modelGeneration, contextData);
                FillSteerings(cars, contextData);
                FillColourCombinations(cars,modelGeneration, contextData, isPreview);
                FillCars(cars, contextData);
                FillGrades(cars, modelGeneration, contextData);
                FillCarAssets(cars, contextData, modelGeneration);
                FillGradeEquipment(grades, modelGeneration, contextData, isPreview);
                FillSubModels(grades,cars, modelGeneration, contextData, isPreview);
                FillGradePacks(grades, contextData);

                context.TimeFrames[language] = _timeFrameMapper.GetTimeFrames(language, context);
            }
        }

        private static IEnumerable<KeyValuePair<string, Tuple<ModelGeneration, Model>>> GetModelGenerationForEachLanguage(String brand, String countryCode, Guid generationID)
        {
            // Is ensuring a context can be retrieved by setting to the know global context necessary?
            MyContext.SetSystemContext("Toyota", "ZZ", "en");

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
                .ToDictionary();
        }

        private void FillCarAssets(IEnumerable<Car> cars, ContextData contextData, ModelGeneration modelGeneration)
        {
            foreach (var car in cars)
            {
                FillCarAssets(car, contextData, modelGeneration, car.Generation.BodyTypes[car.BodyTypeID]);
                FillCarAssets(car, contextData, modelGeneration, car.Generation.Engines[car.EngineID]);
                FillCarAssets(car, contextData, modelGeneration, car.Generation.Grades[car.GradeID]);
            }
        }

        private void FillCarAssets(Car car, ContextData contextData, ModelGeneration modelGeneration, IHasAssetSet objectWithAssetSet)
        {
            var objectAssetsOnCarLevel = GetObjectAssetsOnCarLevel(car, modelGeneration, objectWithAssetSet);
            var objectAssetsOnGenerationLevel = GetObjectAssetsOnGenerationLevel(objectWithAssetSet.GetObjectID(), contextData.Assets);

            var allCarAssetsForThisObject = objectAssetsOnCarLevel.Concat(objectAssetsOnGenerationLevel).ToList();

            contextData.CarAssets[car.ID].Add(objectWithAssetSet.GetObjectID(), allCarAssetsForThisObject);
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

        private IEnumerable<KeyValuePair<Guid, IList<Asset>>> GetSubModelAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.SubModels.ToDictionary(
                subModel => subModel.ID,
                subModel => (IList<Asset>)subModel.AssetSet.Assets.GetGenerationAssets()
                    .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private IEnumerable<KeyValuePair<Guid, IList<Asset>>> GetColourCombinationAssets(ModelGeneration modelGeneration)
        {
            return GetExteriorColourAssets(modelGeneration).Concat(GetUpholsteryAssets(modelGeneration));
        }

        private IEnumerable<KeyValuePair<Guid, IList<Asset>>> GetExteriorColourAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.ColourCombinations
                                  .ExteriorColours()
                                  .ToDictionary(
                                    colour => colour.ID,
                                    colour => (IList<Asset>)colour.AssetSet.Assets.GetGenerationAssets()
                                        .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private IEnumerable<KeyValuePair<Guid, IList<Asset>>> GetUpholsteryAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.ColourCombinations
                                  .Upholsteries()
                                  .ToDictionary(
                                    upholstery => upholstery.ID,
                                    upholstery => (IList<Asset>)upholstery.AssetSet.Assets.GetGenerationAssets()
                                        .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private IEnumerable<KeyValuePair<Guid, IList<Asset>>> GetGradeAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.Grades.ToDictionary(
                grade => grade.ID,
                grade => (IList<Asset>)grade.AssetSet.Assets.GetGenerationAssets()
                    .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private IEnumerable<KeyValuePair<Guid, IList<Asset>>> GetTransmissionAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.Transmissions.ToDictionary(
                transmission => transmission.ID,
                transmission => (IList<Asset>)transmission.AssetSet.Assets.GetGenerationAssets()
                        .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private IEnumerable<KeyValuePair<Guid, IList<Asset>>> GetBodyTypeAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.BodyTypes.ToDictionary(
                bodytype => bodytype.ID,
                bodytype => (IList<Asset>)bodytype.AssetSet.Assets.GetGenerationAssets()
                        .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private IEnumerable<KeyValuePair<Guid, IList<Asset>>> GetEngineAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.Engines.ToDictionary(
                engine => engine.ID,
                engine => (IList<Asset>)engine.AssetSet.Assets
                                         .GetGenerationAssets()
                                         .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private IEnumerable<KeyValuePair<Guid, IList<Asset>>> GetWheelDriveAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.WheelDrives.ToDictionary(
                wheelDrive => wheelDrive.ID,
                wheelDrive => (IList<Asset>)wheelDrive.AssetSet.Assets
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
            foreach (var colourCombination in colourCombinations) {
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

                var mappedSubModel = _subModelMapper.MapSubModel(grades, modelGenerationSubModel, contextData, isPreview);
                
                contextData.SubModels.Add(mappedSubModel);

                FillSubModelGradeEquipment(modelGenerationSubModel,contextData);

                AddSubModelToCar(cars, contextData, subModelId, mappedSubModel);
            }
        }

        private void FillSubModelGradeEquipment(ModelGenerationSubModel modelGenerationSubModel, ContextData contextData)
        {
            contextData.SubModelGradeEquipment.Add(modelGenerationSubModel.ID,_equipmentMapper.MapSubModelGradeEquipment(modelGenerationSubModel,contextData));
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

        void FillGradeEquipment(IEnumerable<ModelGenerationGrade> grades, ModelGeneration modelGeneration, ContextData data, Boolean isPreview)
        {
            var crossModelAccessories = EquipmentItems.GetEquipmentItems(Administration.Enums.EquipmentType.Accessory);
            var crossModelOptions = EquipmentItems.GetEquipmentItems(Administration.Enums.EquipmentType.Option);
            var categories = EquipmentCategories.GetEquipmentCategories();

            foreach (var grade in grades)
            {
                var gradeCars = grade.Cars().ToList();

                var accessories = grade.Equipment.OfType<ModelGenerationGradeAccessory>()
                    .Where(accessory => gradeCars.Any(car => car.Equipment[accessory.ID] != null && car.Equipment[accessory.ID].Availability != Administration.Enums.Availability.NotAvailable))
                    .Select(accessory =>
                        _equipmentMapper.MapGradeAccessory(
                            accessory,
                            (ModelGenerationAccessory)modelGeneration.Equipment[accessory.ID],
                            (Administration.Accessory)crossModelAccessories[accessory.ID],
                            categories,
                            gradeCars,
                            isPreview)).ToList();

                var options = grade.Equipment.OfType<ModelGenerationGradeOption>()
                        .Where(option => gradeCars.Any(car => car.Equipment[option.ID] != null && car.Equipment[option.ID].Availability != Administration.Enums.Availability.NotAvailable))
                        .Select(option =>
                            _equipmentMapper.MapGradeOption(
                            option,
                            (Administration.ModelGenerationOption)modelGeneration.Equipment[option.ID],
                            (Administration.Option)crossModelOptions[option.ID],
                            categories,
                            gradeCars,
                            isPreview)).ToList();

                data.GradeEquipment.Add(grade.ID, new GradeEquipment
                {
                    Accessories = accessories,
                    Options = options
                });
            }
        }

        private void FillGradePacks(IEnumerable<ModelGenerationGrade> grades, ContextData contextData)
        {
            foreach (var grade in grades)
            {
                var gradeCars = grade.Cars().ToList();
                var gradePacks = grade.Packs;
                var generationPacks = grade.Generation.Packs;

                var mappedGradePacks = gradePacks
                    .Select(gradePack => _packMapper.MapGradePack(gradePack, generationPacks[gradePack.ID], gradeCars))
                    .Where(gradePack => gradePack.OptionalOn.Any() || gradePack.StandardOn.Any()) // do not publish packs that are not available on any car
                    .ToList();

                contextData.GradePacks.Add(grade.ID, mappedGradePacks);
            }
        }
    }
}
