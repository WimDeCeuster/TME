using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using DBCar = TME.CarConfigurator.Administration.Car;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Publisher
{
    public class Mapper : IMapper
    {
        public IContext Map(String brand, String country, Guid generationID, ICarDbModelGenerationFinder generationFinder, IContext context)
        {
            var data = generationFinder.GetModelGeneration(brand, country, generationID);
            var isPreview = context.DataSubset == PublicationDataSubset.Preview;

            foreach (var entry in data) {
                var contextData = new ContextData();
                var modelGeneration = entry.Value.Item1;
                var model = entry.Value.Item2;
                var language = entry.Key;

                context.ModelGenerations[language] = modelGeneration;
                context.ContextData[language] = contextData;

                // fill contextData
                var generation = AutoMapper.Mapper.Map<Generation>(modelGeneration);
                contextData.Generations.Add(generation);
                contextData.Models.Add(AutoMapper.Mapper.Map<Model>(model));

                FillModelLinks(model, modelGeneration, generation, brand, country, language, isPreview);
                FillGenerationAssets(modelGeneration, generation, brand, country, language);
                FillGenerationBodyTypes(modelGeneration, contextData);
                FillGenerationEngines(modelGeneration, contextData);
                FillCars(modelGeneration, contextData);

                context.TimeFrames[language] = GetTimeFrames(language, context);
            }

            return context;
        }

        private void FillGenerationAssets(Administration.ModelGeneration modelGeneration, Generation generation, string brand, string country, string language)
        {
            Administration.MyContext.SetSystemContext(brand, country, language);
            generation.Assets = FillAssetList(modelGeneration);
        }

        private List<Asset> FillAssetList(Administration.ModelGeneration modelGeneration)
        {
            var assetList = new List<Asset>();
            foreach (var asset in modelGeneration.Assets)
            {
                var newAsset = AutoMapper.Mapper.Map<Asset>(asset);
                var assetDetails = Administration.Assets.DetailedAssetInfo.GetDetailedAssetInfo(asset.ID);
                assetList.Add(AutoMapper.Mapper.Map(assetDetails, newAsset));
            }
            return assetList;
        }

        void FillModelLinks(Administration.Model model, Administration.ModelGeneration modelGeneration, Generation generation, String brand, String country, String language, Boolean isPreview)
        {
            Administration.MyContext.SetSystemContext(brand, country, language);
            generation.Links = model.Links.Where(link => link.Type.CarConfiguratorversionID == modelGeneration.ActiveCarConfiguratorVersion.ID ||
                                                         link.Type.CarConfiguratorversionID == 0)
                                          .Select(link => GetLink(link, country, language, isPreview))
                                          .Where(x => x != null)
                                          .ToList();
        }

        void FillCars(Administration.ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var car in modelGeneration.Cars)
            {
                var bodyType = contextData.GenerationBodyTypes.Single(type => type.ID == car.BodyTypeID);
                var engine = contextData.GenerationEngines.Single(eng => eng.ID == car.EngineID);
                contextData.Cars.Add(MapCar(car, bodyType, engine));
            }
        }

        void FillGenerationBodyTypes(Administration.ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var bodyType in modelGeneration.BodyTypes) { 
                var globalBodyType = Administration.BodyTypes.GetBodyTypes()[bodyType.ID];
                contextData.GenerationBodyTypes.Add(MapBodyType(bodyType, globalBodyType));
            }
        }

        void FillGenerationEngines(Administration.ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var engine in modelGeneration.Engines)
            {
                var globalEngine = Administration.Engines.GetEngines()[engine.ID];
                var engineCategory = Administration.EngineCategories.GetEngineCategories()[globalEngine.Category.ID];
                var fuelType = Administration.FuelTypes.GetFuelTypes()[globalEngine.Type.FuelType.ID];
                contextData.GenerationEngines.Add(MapEngine(engine, globalEngine, engineCategory, fuelType));
            }
        }

        Link GetLink(Administration.Link link, String countryCode, String languageCode, Boolean isPreview)
        {
            var baseLink = Administration.BaseLinks.GetBaseLinks(link.Type, isPreview)
                                                   .SingleOrDefault(baseLnk => baseLnk.CountryCode == countryCode && baseLnk.LanguageCode == languageCode);

            return new Link {
                ID = link.Type.ID,
                Label = link.Label,
                Name = link.Type.Name,
                Url = GetUrl(baseLink, link)
            };
        }

        String GetUrl(Administration.BaseLink baseLink, Administration.Link link) {
            if (baseLink == null || String.IsNullOrWhiteSpace(baseLink.Url))
                return link.UrlPart;
            if (link.UrlPart.StartsWith("http://") || link.UrlPart.StartsWith("https://"))
                return link.UrlPart;
            return baseLink.Url + "/" + link.UrlPart;
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

        BodyType MapBodyType(Administration.ModelGenerationBodyType generationBodyType, Administration.BodyType globalBodyType)
        {
            return Map<BodyType>(generationBodyType, globalBodyType);
        }

        Engine MapEngine(Administration.ModelGenerationEngine generationEngine, Administration.Engine globalEngine, Administration.EngineCategory engineCategory, Administration.FuelType fuelType)
        {
            var engine = AutoMapper.Mapper.Map<Engine>(generationEngine);
            AutoMapper.Mapper.Map(globalEngine, engine);

            engine.Category = AutoMapper.Mapper.Map<EngineCategory>(engineCategory);
            engine.Type.FuelType = AutoMapper.Mapper.Map<FuelType>(fuelType);
            
            return engine;
        }

        Car MapCar(Administration.Car car, BodyType bodyType, Engine engine)
        {
            var mappedCar = AutoMapper.Mapper.Map<Car>(car);
            mappedCar.BodyType = bodyType;
            mappedCar.Engine = engine;
            //mappedCar.Transmission = transmission;

            return mappedCar;
        }


        TDestination Map<TDestination>(object source1, object source2)
        {
            return (TDestination)AutoMapper.Mapper.Map(source2, AutoMapper.Mapper.Map<TDestination>(source1), source2.GetType(), typeof(TDestination));
        }
    }
}
