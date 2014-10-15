using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
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
        readonly ICarMapper _carMapper;

        public Mapper(IModelMapper modelMapper, IGenerationMapper generationMapper, IBodyTypeMapper bodyTypeMapper, IEngineMapper engineMapper, ICarMapper carMapper)
        {
            if (modelMapper == null) throw new ArgumentNullException("modelMapper");
            if (generationMapper == null) throw new ArgumentNullException("generationMapper");
            if (bodyTypeMapper == null) throw new ArgumentNullException("bodyTypeMapper");
            if (engineMapper == null) throw new ArgumentNullException("engineMapper");
            if (carMapper == null) throw new ArgumentNullException("carMapper");

            _modelMapper = modelMapper;
            _generationMapper = generationMapper;
            _bodyTypeMapper = bodyTypeMapper;
            _engineMapper = engineMapper;
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

                Administration.MyContext.SetSystemContext(brand, country, language);

                context.ModelGenerations[language] = modelGeneration;
                context.ContextData[language] = contextData;

                // fill contextData
                var generation = _generationMapper.MapGeneration(model, modelGeneration, brand, country, language, isPreview);
                contextData.Generations.Add(generation);
                contextData.Models.Add(_modelMapper.MapModel(model));

                FillBodyTypes(modelGeneration, contextData);
                FillGenerationAssets(modelGeneration, generation,contextData, brand, country, language);
                FillGenerationEngines(modelGeneration, contextData);
                FillCars(modelGeneration, contextData);

                context.TimeFrames[language] = GetTimeFrames(language, context);
            }

            return context;
        }

        private void FillGenerationAssets(Administration.ModelGeneration modelGeneration, Generation generation,ContextData contextData, string brand, string country, string language)
            contextData.Assets = FillBodyTypeAssets(modelGeneration);
        }

        private Dictionary<Guid,List<Asset>> FillBodyTypeAssets(ModelGeneration modelGeneration)
        {
            var assetList = new List<Asset>();
            var assetDictionary = new Dictionary<Guid,List<Asset>>();
            foreach (var modelGenerationBodyType in modelGeneration.BodyTypes)
            {
                var newModelGenerationBodyType = AutoMapper.Mapper.Map<BodyType>(modelGenerationBodyType);
                foreach (var asset in modelGenerationBodyType.AssetSet.Assets)
                {
                    var newAsset = AutoMapper.Mapper.Map<Asset>(asset);
                    assetList.Add(newAsset);
                }
                assetDictionary.Add(newModelGenerationBodyType.ID,assetList);
            }
            return assetDictionary;
        void FillCars(Administration.ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var car in modelGeneration.Cars)
            {
                var bodyType = contextData.GenerationBodyTypes.Single(type => type.ID == car.BodyTypeID);
                var engine = contextData.GenerationEngines.Single(eng => eng.ID == car.EngineID);
                contextData.Cars.Add(_carMapper.MapCar(car, bodyType, engine));
            }
        }

        void FillBodyTypes(Administration.ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var bodyType in modelGeneration.BodyTypes)
                contextData.GenerationBodyTypes.Add(_bodyTypeMapper.MapBodyType(bodyType));
        }

        void FillGenerationEngines(Administration.ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var engine in modelGeneration.Engines)
                contextData.GenerationEngines.Add(_engineMapper.MapEngine(engine));
        }

        IReadOnlyList<TimeFrame> GetTimeFrames(String language, IContext context)
        {
            var generation = context.ModelGenerations[language];
            var cars = context.ContextData[language].Cars;

            //For preview, return only 1 Min/Max TimeFrame with all cars
            if (context.DataSubset == PublicationDataSubset.Preview)
                return new List<TimeFrame> { new TimeFrame(DateTime.MinValue, DateTime.MaxValue, cars.ToList()) };

            var timeFrames = new List<TimeFrame>();

            var timeProjection = generation.Cars.SelectMany(car => new[] {
                                                    new { Date = car.LineOffFromDate, Open = true, Car = car },
                                                    new { Date = car.LineOffToDate, Open = false, Car = car }
                                                })
                                                .OrderBy(point => point.Date);

            Func<DBCar, Car> MapCar = dbCar => cars.Single(car => car.ID == dbCar.ID);

            var openCars = new List<DBCar>();
            DateTime? openDate = null;
            DateTime closeDate;
            foreach (var point in timeProjection)
            {
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
