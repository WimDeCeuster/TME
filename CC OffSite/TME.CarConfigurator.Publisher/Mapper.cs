using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Interfaces;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.Mappers.Exceptions;
using TME.CarConfigurator.Repository.Objects.Equipment;
using Asset = TME.CarConfigurator.Repository.Objects.Assets.Asset;
using Car = TME.CarConfigurator.Repository.Objects.Car;

namespace TME.CarConfigurator.Publisher
{
    public class Mapper : IMapper
    {
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
        //readonly IEquipmentMapper _equipmentMapper;

        public Mapper(
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
            ISubModelMapper subModelMapper)
        {
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
            //if (equipmentMapper == null) throw new ArgumentNullException("equipmentMapper");

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
            //_equipmentMapper = equipmentMapper;
        }

        public IContext Map(String brand, String country, Guid generationID, ICarDbModelGenerationFinder generationFinder, IContext context)
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
                FillSubModels(cars, modelGeneration, contextData, isPreview);
                FillCars(cars, contextData);
                FillGrades(cars, modelGeneration, contextData);
                FillCarAssets(cars, contextData, modelGeneration);

                context.TimeFrames[language] = GetTimeFrames(language, context);
            }

            return context;
        }

        private void FillAssets(ModelGeneration modelGeneration, ContextData contextData)
        {
            contextData.Assets =
                GetBodyTypeAssets(modelGeneration)
                .Concat(GetEngineAssets(modelGeneration))
                .Concat(GetTransmissionAssets(modelGeneration))
                .Concat(GetWheelDriveAssets(modelGeneration))
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
            if (!assets.ContainsKey(objectId)) 
                return new List<Asset>();

            // TODO: check with Wim if correct assumption
            return assets[objectId].Where(a => !string.IsNullOrEmpty(a.AssetType.View)); // on car level, we only need generation assets that can be used in the car configurator spin
            
        }

        private Dictionary<Guid, List<Asset>> GetSubModelAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.SubModels.ToDictionary(
                subModel => subModel.ID,
                subModel => subModel.AssetSet.Assets.GetGenerationAssets()
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
                var subModel = contextData.SubModels.SingleOrDefault(sub => sub.ID == car.SubModelID);
                contextData.CarAssets.Add(car.ID, new Dictionary<Guid, IList<Asset>>());
                contextData.Cars.Add(_carMapper.MapCar(car, bodyType, engine, transmission, wheelDrive, steering, subModel));
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
            foreach (var modelGenerationSubModel in modelGeneration.SubModels.Where(submodel => cars.Any(car => car.SubModelID == submodel.ID)))
                contextData.SubModels.Add(_subModelMapper.MapSubModel(modelGenerationSubModel, cars, isPreview));
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

        void FillSteerings(IEnumerable<Administration.Car> cars, ContextData contextData)
        {
            var steerings = cars.Select(car => Steerings.GetSteerings()[car.SteeringID]).Distinct();

            foreach (var steering in steerings)
                contextData.Steerings.Add(_steeringMapper.MapSteering(steering));
        }

        static IReadOnlyList<TimeFrame> GetTimeFrames(String language, IContext context)
        {
            var contextData = context.ContextData[language];
            var cars = contextData.Cars;

            //For preview, return only 1 Min/Max TimeFrame with all data
            if (context.DataSubset == PublicationDataSubset.Preview)
                return new List<TimeFrame> {
                    new TimeFrame(
                        DateTime.MinValue,
                        DateTime.MaxValue,
                        cars.ToList(),
                        contextData.BodyTypes.ToList(),
                        contextData.Engines.ToList(),
                        contextData.WheelDrives.ToList(),
                        contextData.Transmissions.ToList(),
                        contextData.Steerings.ToList(),
                        contextData.Grades.ToList(),
                        contextData.GradeEquipmentItems.ToList(),
                        contextData.SubModels.ToList())
                };

            var timeFrames = new List<TimeFrame>();

            var timeProjection = context.ModelGenerations[language].Cars.Where(car => car.Approved)
                                                .SelectMany(car => new[] {
                                                    new { Date = car.LineOffFromDate, Open = true, Car = car },
                                                    new { Date = car.LineOffToDate, Open = false, Car = car }
                                                })
                                                .OrderBy(point => point.Date);


            var openCars = new List<Administration.Car>();

            DateTime? openDate = null;
            foreach (var point in timeProjection)
            {
                DateTime closeDate;
                if (point.Open)
                {
                    if (openDate != null)
                    {
                        closeDate = point.Date;
                        AddTimeFrameIfRelevant(openDate, closeDate, timeFrames, openCars, context.ContextData[language]);
                    }

                    openCars.Add(point.Car);
                    openDate = point.Date;

                    continue;
                }

                closeDate = point.Date;

                AddTimeFrameIfRelevant(openDate, closeDate, timeFrames, openCars, context.ContextData[language]);

                openCars.Remove(point.Car);
                openDate = openCars.Any() ? (DateTime?)point.Date : null;
            }

            return timeFrames;
        }

        private static void AddTimeFrameIfRelevant(DateTime? openDate, DateTime closeDate, ICollection<TimeFrame> timeFrames, IReadOnlyList<Administration.Car> openCars, ContextData contextData)
        {
            // time lines with identical from/until can occur when multiple line off dates fall on the same point
            // these "empty" time lines can simply be ignored (though the openCars logic is still relevant)
            if (openDate == closeDate) return;

            if (openDate == null)
                throw new CorruptDataException("The open date could not be retrieved, could not create timeframe");

            Func<Administration.Car, Car> getCar = dbCar => contextData.Cars.Single(car => car.ID == dbCar.ID);
            Func<Administration.Car, Repository.Objects.BodyType> getBodyType = dbCar => contextData.BodyTypes.Single(bodyType => bodyType.ID == dbCar.BodyTypeID);
            Func<Administration.Car, Repository.Objects.Engine> getEngine = dbCar => contextData.Engines.Single(engine => engine.ID == dbCar.EngineID);
            Func<Administration.Car, Repository.Objects.WheelDrive> getWheelDrive = dbCar => contextData.WheelDrives.Single(wheelDrive => wheelDrive.ID == dbCar.WheelDriveID);
            Func<Administration.Car, Repository.Objects.Transmission> getTransmission = dbCar => contextData.Transmissions.Single(tranmission => tranmission.ID == dbCar.TransmissionID);
            Func<Administration.Car, Repository.Objects.Steering> getSteering = dbCar => contextData.Steerings.Single(steering => steering.ID == dbCar.SteeringID);
            Func<Administration.Car, Repository.Objects.Grade> getGrade = dbCar => contextData.Grades.Single(grade => grade.ID == dbCar.GradeID);
            Func<Administration.Car, GradeEquipmentItem> getGradeEquipmentItem = dbCar => contextData.GradeEquipmentItems.Single(gradeEquipmentItem => dbCar.Equipment.Any(equipment => gradeEquipmentItem.ID == equipment.ID));
            Func<Administration.Car, Repository.Objects.SubModel> getSubModel = dbCar => contextData.SubModels.SingleOrDefault(subModel => subModel.ID == dbCar.SubModelID);

            var cars = openCars.Select(getCar).ToList();
            var bodyTypes = openCars.Select(getBodyType).Distinct().ToList();
            var engines = openCars.Select(getEngine).Distinct().ToList();
            var wheelDrives = openCars.Select(getWheelDrive).Distinct().ToList();
            var transmissions = openCars.Select(getTransmission).Distinct().ToList();
            var steerings = openCars.Select(getSteering).Distinct().ToList();
            var grades = openCars.Select(getGrade).Distinct().ToList();
            var gradeEquipmentItems = openCars.Select(getGradeEquipmentItem).Distinct().ToList();
            var subModels = openCars.Select(getSubModel).Distinct().Where(subModel => subModel != null).ToList();

            timeFrames.Add(new TimeFrame(
                openDate.Value,
                closeDate,
                cars,
                bodyTypes,
                engines,
                wheelDrives,
                transmissions,
                steerings,
                grades,
                gradeEquipmentItems,
                subModels));
        }
    }
}
