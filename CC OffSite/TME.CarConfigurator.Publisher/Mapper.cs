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
using TME.CarConfigurator.Repository.Objects.Equipment;
using Asset = TME.CarConfigurator.Repository.Objects.Assets.Asset;
using Car = TME.CarConfigurator.Administration.Car;

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
            IPackMapper packMapper)
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
        }

        public Task MapAsync(string brand, string country, Guid generationID, ICarDbModelGenerationFinder generationFinder, IContext context)
        {
            return Task.Run(() => Map(brand, country, generationID, generationFinder, context));
        }

        private void Map(string brand, string country, Guid generationID, ICarDbModelGenerationFinder generationFinder, IContext context)
        {
            var data = generationFinder.GetModelGeneration(brand, country, generationID);
            var isPreview = context.DataSubset == PublicationDataSubset.Preview;

            foreach (var entry in data)
            {
                var contextData = new ContextData();
                var modelGeneration = entry.Value.Item1;
                var model = entry.Value.Item2;
                var language = entry.Key;

                MyContext.SetSystemContext(brand, country, language);

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
                FillCars(cars, contextData);
                FillGrades(cars, modelGeneration, contextData);
                FillSubModels(cars, modelGeneration, contextData, isPreview);
                FillCarAssets(cars, contextData, modelGeneration);
                FillGradeEquipment(grades, modelGeneration, contextData, isPreview);
                FillGradePacks(grades, contextData);

                context.TimeFrames[language] = _timeFrameMapper.GetTimeFrames(language, context);
            }
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
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value);
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

        void FillWheelDrives(IEnumerable<Car> cars, ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var wheelDrive in modelGeneration.WheelDrives.Where(wheelDrive => cars.Any(car => car.WheelDriveID == wheelDrive.ID)))
                contextData.WheelDrives.Add(_wheelDriveMapper.MapWheelDrive(wheelDrive));
        }

        private void FillSubModels(IList<Car> cars, ModelGeneration modelGeneration, ContextData contextData, bool isPreview)
        {
            var applicableSubModels = modelGeneration.SubModels.Where(submodel => cars.Any(car => car.SubModelID == submodel.ID)).ToList();

            foreach (var modelGenerationSubModel in applicableSubModels)
                contextData.SubModels.Add(_subModelMapper.MapSubModel(modelGenerationSubModel, contextData, isPreview));

            PutSubModelOnApplicableCars(cars, contextData, applicableSubModels);
        }

        private static void PutSubModelOnApplicableCars(IList<Car> cars, ContextData contextData, List<ModelGenerationSubModel> applicableSubModels)
        {
            foreach (var modelGenerationSubModel in applicableSubModels)
            {
                var subModelId = modelGenerationSubModel.ID;

                var mappedSubModel = contextData.SubModels.Single(contextSubmodel => subModelId == contextSubmodel.ID);

                var applicableCars =
                    cars.Where(car => car.SubModelID == subModelId)
                        .Select(car => contextData.Cars.Single(contextCar => contextCar.ID == car.ID));

                foreach (var applicableCar in applicableCars)
                    applicableCar.SubModel = mappedSubModel;
            }
        }

        void FillGrades(IList<Car> cars, ModelGeneration modelGeneration, ContextData contextData)
        {
            var applicableGrades = modelGeneration.Grades.Where(grade => cars.Any(car => car.GradeID == grade.ID)).ToArray();

            foreach (var grade in applicableGrades)
                contextData.Grades.Add(_gradeMapper.MapGrade(grade, contextData.Cars));

            PutGradesOnApplicableCars(cars, contextData, applicableGrades);
        }

        private static void PutGradesOnApplicableCars(IList<Car> cars, ContextData contextData,
            ModelGenerationGrade[] applicableGrades)
        {
            foreach (var grade in applicableGrades)
            {
                var gradeID = grade.ID;

                var mappedGrade = contextData.Grades.Single(contextGrade => gradeID == contextGrade.ID);

                var applicableCars = cars.Where(car => car.GradeID == gradeID)
                    .Select(car => contextData.Cars.Single(contextCar => contextCar.ID == car.ID));
                foreach (var applicableCar in applicableCars)
                    applicableCar.Grade = mappedGrade;
            }
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
                var accessories = grade.Equipment.OfType<ModelGenerationGradeAccessory>()
                    .Select(accessory =>
                        _equipmentMapper.MapGradeAccessory(
                            accessory,
                            (ModelGenerationAccessory)modelGeneration.Equipment.Single(equipment => equipment.ID == accessory.ID),
                            (Administration.Accessory)crossModelAccessories[accessory.ID],
                            categories,
                            isPreview));

                var options = grade.Equipment.OfType<ModelGenerationGradeOption>()
                    .Select(option =>
                        _equipmentMapper.MapGradeOption(
                            option,
                            (ModelGenerationOption)modelGeneration.Equipment.Single(equipment => equipment.ID == option.ID),
                            (Administration.Option)crossModelOptions[option.ID],
                            categories,
                            isPreview));

                data.GradeEquipment.Add(grade.ID, new GradeEquipment
                {
                    Accessories = accessories.ToList(),
                    Options = options.ToList()
                });
            }
        }

        private void FillGradePacks(IEnumerable<ModelGenerationGrade> grades, ContextData contextData)
        {
            foreach (var grade in grades)
            {
                var gradePacks = grade.Packs;
                var generationPacks = grade.Generation.Packs;

                var mappedGradePacks = gradePacks.Select(gp => _packMapper.MapGradePack(gp, generationPacks[gp.ID])).ToList();

                contextData.GradePacks.Add(grade.ID, mappedGradePacks);
            }
        }
    }
}
