using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.Mappers;
using TME.CarConfigurator.Repository.Objects;
using DBCar = TME.CarConfigurator.Administration.Car;
using Asset = TME.CarConfigurator.Repository.Objects.Assets.Asset;
using BodyType = TME.CarConfigurator.Repository.Objects.BodyType;
using Car = TME.CarConfigurator.Repository.Objects.Car;
using Engine = TME.CarConfigurator.Repository.Objects.Engine;
using EngineCategory = TME.CarConfigurator.Repository.Objects.EngineCategory;
using FuelType = TME.CarConfigurator.Repository.Objects.FuelType;
using Link = TME.CarConfigurator.Repository.Objects.Link;
using Model = TME.CarConfigurator.Repository.Objects.Model;

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
        readonly ICarMapper _carMapper;
        readonly IAssetMapper _assetMapper;

        public Mapper(IModelMapper modelMapper,
                      IGenerationMapper generationMapper,
                      IBodyTypeMapper bodyTypeMapper,
                      IEngineMapper engineMapper,
                      ITransmissionMapper transmissionMapper,
                      IWheelDriveMapper wheelDriveMapper,
                      ICarMapper carMapper,
                      IAssetMapper assetMapper)
        {
            if (modelMapper == null) throw new ArgumentNullException("modelMapper");
            if (generationMapper == null) throw new ArgumentNullException("generationMapper");
            if (bodyTypeMapper == null) throw new ArgumentNullException("bodyTypeMapper");
            if (engineMapper == null) throw new ArgumentNullException("engineMapper");
            if (transmissionMapper == null) throw new ArgumentNullException("transmissionMapper");
            if (wheelDriveMapper == null) throw new ArgumentNullException("wheelDriveMapper");
            if (carMapper == null) throw new ArgumentNullException("carMapper");
            if (assetMapper == null) throw new ArgumentNullException("assetMapper");

            _modelMapper = modelMapper;
            _assetMapper = assetMapper;
            _generationMapper = generationMapper;
            _bodyTypeMapper = bodyTypeMapper;
            _engineMapper = engineMapper;
            _transmissionMapper = transmissionMapper;
            _wheelDriveMapper = wheelDriveMapper;
            _carMapper = carMapper;
        }

        public IContext Map(String brand, String country, Guid generationID, ICarDbModelGenerationFinder generationFinder, IContext context)
        {
            var data = generationFinder.GetModelGeneration(brand, country, generationID);
            var isPreview = context.DataSubset == PublicationDataSubset.Preview;

            foreach (var entry in data) {
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

                FillBodyTypes(modelGeneration, contextData);
                FillEngines(modelGeneration, contextData);
                FillObjectAssets(modelGeneration,contextData);
                FillTransmissions(modelGeneration, contextData);
                FillWheelDrives(modelGeneration, contextData);

                var cars = modelGeneration.Cars.Where(car => isPreview || car.Approved).ToList();
                FillCars(cars, contextData);

                context.TimeFrames[language] = GetTimeFrames(language, context);
            }

            return context;
        }

        private void FillObjectAssets(ModelGeneration modelGeneration,ContextData contextData){
            contextData.Assets = 
                GetBodyTypeAssets(modelGeneration)
                .Concat(GetEngineAssets(modelGeneration))
                .Concat(GetWheelDriveAssets(modelGeneration))
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value);
        }

        private Dictionary<Guid,List<Asset>> GetBodyTypeAssets(ModelGeneration modelGeneration)
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
                contextData.Cars.Add(_carMapper.MapCar(car, bodyType, engine, transmission, wheelDrive));
            }
        }

        void FillBodyTypes(ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var bodyType in modelGeneration.BodyTypes)
                contextData.BodyTypes.Add(_bodyTypeMapper.MapBodyType(bodyType));
        }

        void FillEngines(ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var engine in modelGeneration.Engines)
                contextData.Engines.Add(_engineMapper.MapEngine(engine));
        }

        void FillTransmissions(ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var transmission in modelGeneration.Transmissions)
                contextData.Transmissions.Add(_transmissionMapper.MapTransmission(transmission));
        }

        void FillWheelDrives(ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var wheelDrive in modelGeneration.WheelDrives)
                contextData.WheelDrives.Add(_wheelDriveMapper.MapWheelDrive(wheelDrive));
        }

        IReadOnlyList<TimeFrame> GetTimeFrames(String language, IContext context)
        {
            var generation = context.ModelGenerations[language];
            var cars = context.ContextData[language].Cars;

            //For preview, return only 1 Min/Max TimeFrame with all cars
            if (context.DataSubset == PublicationDataSubset.Preview)
                return new List<TimeFrame> { new TimeFrame(DateTime.MinValue, DateTime.MaxValue, cars.ToList()) };

            var timeFrames = new List<TimeFrame>();

            var timeProjection = generation.Cars.Where(car => car.Approved)
                                                .SelectMany(car => new[] {
                                                    new { Date = car.LineOffFromDate, Open = true, Car = car },
                                                    new { Date = car.LineOffToDate, Open = false, Car = car }
                                                })
                                                .OrderBy(point => point.Date);

            Func<DBCar, Car> MapCar = dbCar => cars.Single(car => car.ID == dbCar.ID);

            var openCars = new List<DBCar>();
            DateTime? openDate = null;
            foreach (var point in timeProjection)
            {
                DateTime closeDate;
                if (point.Open)
                {
                    if (openDate != null)
                    { 
                        closeDate = point.Date;
                        if (openDate != closeDate)
                            timeFrames.Add(new TimeFrame(openDate.Value, closeDate, new ReadOnlyCollection<Car>(openCars.Select(MapCar).ToList())));
                    }

                    openCars.Add(point.Car);
                    openDate = point.Date;
                }
                else
                {
                    closeDate = point.Date;

                    // time lines with identical from/until can occur when multiple line off dates fall on the same point
                    // these "empty" time lines can simply be ignored (though the openCars logic is still relevant)
                    if (openDate != closeDate)
                        timeFrames.Add(new TimeFrame(openDate.Value, closeDate, new ReadOnlyCollection<Car>(openCars.Select(MapCar).ToList())));

                    openCars.Remove(point.Car);
                    openDate = openCars.Any() ? (DateTime?)point.Date : null;
                }
            }

            return timeFrames;
        }
    }
}
