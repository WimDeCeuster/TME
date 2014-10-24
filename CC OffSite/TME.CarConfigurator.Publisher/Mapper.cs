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
            IEquipmentMapper equipmentMapper)
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

                var cars = modelGeneration.Cars.Where(car => isPreview || car.Approved).ToList();
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
                FillGradeEquipment(cars, modelGeneration, contextData, isPreview);

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

        private void FillCarAssets(IEnumerable<Administration.Car> cars, ContextData contextData, ModelGeneration modelGeneration)
        {
            foreach (var car in cars)
            {
                FillCarAssets(car, contextData, modelGeneration, car.Generation.BodyTypes[car.BodyTypeID]);
                FillCarAssets(car, contextData, modelGeneration, car.Generation.Engines[car.EngineID]);
                FillCarAssets(car, contextData, modelGeneration, car.Generation.Grades[car.GradeID]);
            }
        }

        private void FillCarAssets(Administration.Car car, ContextData contextData, ModelGeneration modelGeneration, IHasAssetSet objectWithAssetSet)
        {
            var objectAssetsOnCarLevel = GetObjectAssetsOnCarLevel(car, modelGeneration, objectWithAssetSet);
            var objectAssetsOnGenerationLevel = GetObjectAssetsOnGenerationLevel(objectWithAssetSet.GetObjectID(), contextData.Assets);

            var allCarAssetsForThisObject = objectAssetsOnCarLevel.Concat(objectAssetsOnGenerationLevel).ToList();

            contextData.CarAssets[car.ID].Add(objectWithAssetSet.GetObjectID(), allCarAssetsForThisObject);
        }

        private IEnumerable<Asset> GetObjectAssetsOnCarLevel(Administration.Car car, ModelGeneration modelGeneration, IHasAssetSet objectWithAssetSet)
        {
            var objectAssetsOnCarLevel = objectWithAssetSet.AssetSet.Assets.Filter(car);

            return objectAssetsOnCarLevel.Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList();
        }

        private IEnumerable<Asset> GetObjectAssetsOnGenerationLevel(Guid objectId, IDictionary<Guid, List<Asset>> assets)
        {
            return assets.ContainsKey(objectId) ? assets[objectId] : new List<Asset>();
        }

        private Dictionary<Guid, List<Asset>> GetSubModelAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.SubModels.ToDictionary(
                subModel => subModel.ID,
                subModel => subModel.AssetSet.Assets.GetGenerationAssets()
                    .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private Dictionary<Guid, List<Asset>> GetGradeAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.Grades.ToDictionary(
                grade => grade.ID,
                grade => grade.AssetSet.Assets.GetGenerationAssets()
                    .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private Dictionary<Guid, List<Asset>> GetTransmissionAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.Transmissions.ToDictionary(
                transmission => transmission.ID,
                transmission =>
                    transmission.AssetSet.Assets.GetGenerationAssets()
                        .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private Dictionary<Guid, List<Asset>> GetBodyTypeAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.BodyTypes.ToDictionary(
                bodytype => bodytype.ID,
                bodytype =>
                    bodytype.AssetSet.Assets.GetGenerationAssets()
                        .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private Dictionary<Guid, List<Asset>> GetEngineAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.Engines.ToDictionary(
                engine => engine.ID,
                engine => engine.AssetSet.Assets
                                         .GetGenerationAssets()
                                         .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private Dictionary<Guid, List<Asset>> GetWheelDriveAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.WheelDrives.ToDictionary(
                wheelDrive => wheelDrive.ID,
                wheelDrive => wheelDrive.AssetSet.Assets
                                         .GetGenerationAssets()
                                         .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        void FillCars(IEnumerable<Administration.Car> cars, ContextData contextData)
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

        void FillBodyTypes(IEnumerable<Administration.Car> cars, ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var bodyType in modelGeneration.BodyTypes.Where(bodyType => cars.Any(car => car.BodyTypeID == bodyType.ID)))
                contextData.BodyTypes.Add(_bodyTypeMapper.MapBodyType(bodyType));
        }

        void FillEngines(IEnumerable<Administration.Car> cars, ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var engine in modelGeneration.Engines.Where(engine => cars.Any(car => car.EngineID == engine.ID)))
                contextData.Engines.Add(_engineMapper.MapEngine(engine));
        }

        void FillTransmissions(IEnumerable<Administration.Car> cars, ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var transmission in modelGeneration.Transmissions.Where(transmission => cars.Any(car => car.TransmissionID == transmission.ID)))
                contextData.Transmissions.Add(_transmissionMapper.MapTransmission(transmission));
        }

        void FillWheelDrives(IEnumerable<Administration.Car> cars, ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var wheelDrive in modelGeneration.WheelDrives.Where(wheelDrive => cars.Any(car => car.WheelDriveID == wheelDrive.ID)))
                contextData.WheelDrives.Add(_wheelDriveMapper.MapWheelDrive(wheelDrive));
        }

        private void FillSubModels(IList<Administration.Car> cars, ModelGeneration modelGeneration, ContextData contextData, bool isPreview)
        {
            var applicableSubModels =
                modelGeneration.SubModels.Where(submodel => cars.Any(car => car.SubModelID == submodel.ID));
            foreach (var modelGenerationSubModel in applicableSubModels)
                contextData.SubModels.Add(_subModelMapper.MapSubModel(modelGenerationSubModel,contextData, isPreview));

            foreach (var modelGenerationSubModel in applicableSubModels)
            {
                var mappedSubModel =
                    contextData.SubModels.Single(contextSubmodel => modelGenerationSubModel.ID == contextSubmodel.ID);

                var applicableCars = cars.Where(car => car.SubModelID == modelGenerationSubModel.ID)
                    .Select(car => contextData.Cars.Single(contextCar => contextCar.ID == car.ID));

                foreach (var applicableCar in applicableCars)
                    applicableCar.SubModel = mappedSubModel;
            }
        }

        void FillGrades(IList<Administration.Car> cars, ModelGeneration modelGeneration, ContextData contextData)
        {
            var applicableGrades = modelGeneration.Grades.Where(grade => cars.Any(car => car.GradeID == grade.ID)).ToArray();

            foreach (var grade in applicableGrades)
                contextData.Grades.Add(_gradeMapper.MapGrade(grade, contextData.Cars));

            foreach (var grade in applicableGrades)
            {
                var mappedGrade = contextData.Grades.Single(contextGrade => grade.ID == contextGrade.ID);

                var applicableCars = cars.Where(car => car.GradeID == grade.ID)
                                         .Select(car => contextData.Cars.Single(contextCar => contextCar.ID == car.ID));
                foreach (var applicableCar in applicableCars)
                    applicableCar.Grade = mappedGrade;
            }
        }

        void FillGradeEquipment(IEnumerable<Administration.Car> cars, ModelGeneration modelGeneration, ContextData data, Boolean isPreview)
        {
            var grades = modelGeneration.Grades.Where(grade => cars.Any(car => car.GradeID == grade.ID)).ToArray();
            var crossModelAccessories = Administration.EquipmentItems.GetEquipmentItems(Administration.Enums.EquipmentType.Accessory);
            var crossModelOptions = Administration.EquipmentItems.GetEquipmentItems(Administration.Enums.EquipmentType.Option);
            var categories = Administration.EquipmentCategories.GetEquipmentCategories();

            foreach (var grade in grades)
            {
                var accessories = grade.Equipment.OfType<Administration.ModelGenerationGradeAccessory>()
                                                 .Select(accessory =>
                                                     _equipmentMapper.MapGradeAccessory(
                                                        accessory,
                                                        (Administration.ModelGenerationAccessory)modelGeneration.Equipment.Single(equipment => equipment.ID == accessory.ID),
                                                        (Administration.Accessory)crossModelAccessories[accessory.ID],
                                                        categories,
                                                        isPreview));

                var options = grade.Equipment.OfType<Administration.ModelGenerationGradeOption>()
                                                                 .Select(option =>
                                                                     _equipmentMapper.MapGradeOption(
                                                                        option,
                                                                        (Administration.ModelGenerationOption)modelGeneration.Equipment.Single(equipment => equipment.ID == option.ID),
                                                                        (Administration.Option)crossModelOptions[option.ID],
                                                                        categories,
                                                                        isPreview));

                data.GradeEquipments.Add(grade.ID, new GradeEquipment
                {
                    Accessories = accessories.ToList(),
                    Options = options.ToList()
                });
            }
        }

        void FillSteerings(IEnumerable<Administration.Car> cars, ContextData contextData)
        {
            var steerings = cars.Select(car => Steerings.GetSteerings()[car.SteeringID]).Distinct();

            foreach (var steering in steerings)
                contextData.Steerings.Add(_steeringMapper.MapSteering(steering));
        }
    }
}
