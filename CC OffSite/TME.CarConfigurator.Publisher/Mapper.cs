using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Interfaces;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.Mappers;
using TME.CarConfigurator.Publisher.Mappers.Exceptions;
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
                var generation = _generationMapper.MapGeneration(model, modelGeneration, brand, country, language, isPreview);
                contextData.Generations.Add(generation);
                contextData.Models.Add(_modelMapper.MapModel(model));


                var cars = modelGeneration.Cars.Where(car => isPreview || car.Approved).ToList();
                FillAssets(modelGeneration, contextData);
                FillBodyTypes(cars, modelGeneration, contextData);
                FillEngines(cars, modelGeneration, contextData);
                FillTransmissions(cars, modelGeneration, contextData);
                FillWheelDrives(cars, modelGeneration, contextData);
                FillSteerings(cars, contextData);
                FillSubModels(cars, modelGeneration, contextData);
                FillCars(cars, contextData);
                FillCarAssets(cars, contextData, modelGeneration);
                FillGrades(cars, modelGeneration, contextData);

                context.TimeFrames[language] = GetTimeFrames(language, context);
            }

            return context;
        }

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
        }

        public void FillAssets(ModelGeneration modelGeneration, ContextData contextData)
        {
            contextData.Assets =
                GetBodyTypeAssets(modelGeneration)
                .Concat(GetEngineAssets(modelGeneration))
                .Concat(GetTransmissionAssets(modelGeneration))
                .Concat(GetWheelDriveAssets(modelGeneration))
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value);
        }

        private void FillCarAssets(IEnumerable<Administration.Car> cars, ContextData contextData, ModelGeneration modelGeneration)
        {
            foreach (var car in cars)
            {
                var carAssets = contextData.CarAssets[car.ID];
                FillCarAssets(car, carAssets, modelGeneration, car.Generation.BodyTypes[car.BodyTypeID]);
                FillCarAssets(car, carAssets, modelGeneration, car.Generation.Engines[car.EngineID]);
            }
        }

        private void FillCarAssets(Administration.Car car, IDictionary<Guid, IList<Asset>> carAssets, ModelGeneration modelGeneration, IHasAssetSet objectWithAssetSet)
        {
            var objectAssets = objectWithAssetSet.AssetSet.Assets.Filter(car);
            var mappedAssets = objectAssets.Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList();
            carAssets.Add(objectWithAssetSet.GetObjectID(), mappedAssets);
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

        private void FillSubModels(IEnumerable<Administration.Car> cars, ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var modelGenerationSubModel in modelGeneration.SubModels.Where(subModel => cars.Any(car => car.SubModelID == subModel.ID)))
                contextData.SubModels.Add(_subModelMapper.MapSubModel(modelGenerationSubModel));
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

        void FillGrades(IEnumerable<Administration.Car> cars, ModelGeneration modelGeneration, ContextData contextData)
        {
            var applicableGrades =modelGeneration.Grades.Where(grade => cars.Any(car => car.GradeID == grade.ID)).ToArray();

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
            var cars = context.ContextData[language].Cars;

            //For preview, return only 1 Min/Max TimeFrame with all cars
            if (context.DataSubset == PublicationDataSubset.Preview)
                return new List<TimeFrame> { new TimeFrame(DateTime.MinValue, DateTime.MaxValue, cars.ToList()) };

            var timeFrames = new List<TimeFrame>();

            var timeProjection = context.ModelGenerations[language].Cars.Where(car => car.Approved)
                                                .SelectMany(car => new[] {
                                                    new { Date = car.LineOffFromDate, Open = true, Car = car },
                                                    new { Date = car.LineOffToDate, Open = false, Car = car }
                                                })
                                                .OrderBy(point => point.Date);

            Func<Administration.Car, Car> mapCar = dbCar => cars.Single(car => car.ID == dbCar.ID);

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
                        AddTimeFrameIfRelevant(openDate, closeDate, timeFrames, openCars, mapCar);
                    }

                    openCars.Add(point.Car);
                    openDate = point.Date;

                    continue;
                }

                closeDate = point.Date;

                AddTimeFrameIfRelevant(openDate, closeDate, timeFrames, openCars, mapCar);

                openCars.Remove(point.Car);
                openDate = openCars.Any() ? (DateTime?)point.Date : null;
            }

            return timeFrames;
        }

        private static void AddTimeFrameIfRelevant(DateTime? openDate, DateTime closeDate, ICollection<TimeFrame> timeFrames, IEnumerable<Administration.Car> openCars, Func<Administration.Car, Car> mapCar)
        {
            // time lines with identical from/until can occur when multiple line off dates fall on the same point
            // these "empty" time lines can simply be ignored (though the openCars logic is still relevant)
            if (openDate == closeDate) return;

            if (openDate == null)
                throw new CorruptDataException("The open date could not be retrieved, could not create timeframe");

            timeFrames.Add(new TimeFrame(openDate.Value, closeDate,
                new ReadOnlyCollection<Car>(openCars.Select(mapCar).ToList())));
        }
    }
}
