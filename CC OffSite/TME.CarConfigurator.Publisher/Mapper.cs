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
                FillCars(modelGeneration, contextData);
                FillGenerationAssets(modelGeneration, generation, brand, country, language);
                FillGenerationBodyTypes(modelGeneration, contextData);
                FillGenerationEngines(modelGeneration, contextData);

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
                newAsset.Height = Administration.Assets.DetailedAssetInfo.GetDetailedAssetInfo(asset.ID).Height;
                newAsset.Width = Administration.Assets.DetailedAssetInfo.GetDetailedAssetInfo(asset.ID).Width;
                newAsset.PositionX = Administration.Assets.DetailedAssetInfo.GetDetailedAssetInfo(asset.ID).PositionX;
                newAsset.PositionY = Administration.Assets.DetailedAssetInfo.GetDetailedAssetInfo(asset.ID).PositionY;
                
                assetList.Add(newAsset);
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
                contextData.Cars.Add(AutoMapper.Mapper.Map<Car>(car));
        }

        void FillGenerationBodyTypes(Administration.ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var bodyType in modelGeneration.BodyTypes)
                contextData.GenerationBodyTypes.Add(AutoMapper.Mapper.Map<BodyType>(bodyType));
        }

        void FillGenerationEngines(Administration.ModelGeneration modelGeneration, ContextData contextData)
        {
            foreach (var engine in modelGeneration.Engines)
                contextData.GenerationEngines.Add(AutoMapper.Mapper.Map<Engine>(engine));
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
    }
}
