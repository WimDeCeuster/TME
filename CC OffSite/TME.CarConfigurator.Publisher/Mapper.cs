using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Assets;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.Mappers;
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
        readonly ICarMapper _carMapper;
        readonly IAssetMapper _assetMapper;

        public Mapper(IModelMapper modelMapper, IGenerationMapper generationMapper, IBodyTypeMapper bodyTypeMapper, IEngineMapper engineMapper, ITransmissionMapper transmissionMapper, ICarMapper carMapper, IAssetMapper assetMapper)
        {
            if (modelMapper == null) throw new ArgumentNullException("modelMapper");
            if (generationMapper == null) throw new ArgumentNullException("generationMapper");
            if (bodyTypeMapper == null) throw new ArgumentNullException("bodyTypeMapper");
            if (engineMapper == null) throw new ArgumentNullException("engineMapper");
            if (transmissionMapper == null) throw new ArgumentNullException("transmissionMapper");
            if (carMapper == null) throw new ArgumentNullException("carMapper");
            if (assetMapper == null) throw new ArgumentNullException("assetMapper");

            _modelMapper = modelMapper;
            _assetMapper = assetMapper;
            _generationMapper = generationMapper;
            _bodyTypeMapper = bodyTypeMapper;
            _engineMapper = engineMapper;
            _transmissionMapper = transmissionMapper;
            _carMapper = carMapper;
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
                var generation = _generationMapper.MapGeneration(model, modelGeneration, brand, country, language, isPreview);
                contextData.Generations.Add(generation);
                contextData.Models.Add(_modelMapper.MapModel(model));

                FillBodyTypes(modelGeneration, contextData);
                FillEngines(modelGeneration, contextData);
                FillAssets(modelGeneration, contextData);
                FillTransmissions(modelGeneration, contextData);

                var cars = modelGeneration.Cars.Where(car => isPreview || car.Approved).ToList();
                FillCars(cars, contextData);
                FillCarAssets(cars, contextData, modelGeneration);

                context.TimeFrames[language] = GetTimeFrames(language, context);
            }

            return context;
        }

        private void FillCarAssets(IEnumerable<Administration.Car> cars, ContextData contextData, ModelGeneration modelGeneration)
        {
            foreach (var car in cars)
            {
                FillCarBodyTypeAssets(car, contextData.CarAssets[car.ID].ToList(), modelGeneration);
            }
        }

        private void FillCarBodyTypeAssets(Administration.Car car, List<Asset> carAssets, ModelGeneration modelgeneration)
        {
            var bodyTypeAssets = car.Generation.BodyTypes[car.BodyTypeID].AssetSet.Assets;

            var carBodyTypeAssets = FilterAssets(bodyTypeAssets, car);

            var mappedAssets = carBodyTypeAssets.Select(asset => _assetMapper.MapAssetSetAsset(asset, modelgeneration));

            carAssets.AddRange(mappedAssets);
        }

        private static IEnumerable<AssetSetAsset> FilterAssets(IList<AssetSetAsset> assets, Administration.Car car)
        {
            var carAssets = new List<AssetSetAsset>();
            var possibleCarAssets = new Dictionary<string, IList<AssetSetAsset>>();

            foreach (var asset in assets)
            {
                if (!IsAssetOfCar(car, asset) || ThereIsAnotherAssetThatFitsEquallyWellButItAlsoFitsOnTheGradeOfTheCar(car, asset, assets)) continue;

                if (asset.AlwaysInclude)
                {
                    carAssets.Add(asset);
                    continue;
                }

                TryAddAssetToPossibleCarAssets(asset, possibleCarAssets);
            }

            AddPossibleCarAssetsThatAreBetterMatches(carAssets, possibleCarAssets);

            return carAssets;
        }

        private static bool IsAssetOfCar(Administration.Car car, AssetSetAsset asset)
        {
            throw new NotImplementedException();
        }

        private static bool ThereIsAnotherAssetThatFitsEquallyWellButItAlsoFitsOnTheGradeOfTheCar(Administration.Car car, AssetSetAsset asset, IEnumerable<AssetSetAsset> assets)
        {
            if (asset.Grade.ID == car.GradeID) return false; // There is no asset that is the same with a better grade than the car, because this asset already has the same grade. There can (potentially/theoretically) be other assets with the exact same deviation properties, but those shouldn't stop this one from being added. 

            return assets.Any(otherAsset => IsAssetOfCar(car, otherAsset)
                && otherAsset.AssetType.Equals(asset.AssetType)
                && asset.BodyType.IsEmpty() || otherAsset.BodyType.Equals(asset.BodyType) // Only check if bodytype is the same if the current asset has a bodytype, otherwise it can be ignored
                && otherAsset.Engine.Equals(asset.Engine)
                && otherAsset.Transmission.Equals(asset.Transmission)
                && otherAsset.WheelDrive.Equals(asset.WheelDrive)
                && otherAsset.Steering.Equals(asset.Steering)
                && otherAsset.ExteriorColour.Equals(asset.ExteriorColour)
                && otherAsset.Upholstery.Equals(asset.Upholstery)
                && otherAsset.EquipmentItem.Equals(asset.EquipmentItem)
                && otherAsset.Grade.Equals(car.Grade) // if it has all other properties the same, and in addition has the same grade as the car, it is the asset we are looking for
                );
        }

        private static void TryAddAssetToPossibleCarAssets(AssetSetAsset asset, Dictionary<string, IList<AssetSetAsset>> possibleCarAssets)
        {
            throw new NotImplementedException();
        }

        private static void AddPossibleCarAssetsThatAreBetterMatches(List<AssetSetAsset> carAssets, Dictionary<string, IList<AssetSetAsset>> possibleCarAssets)
        {
            RemovePossibleCarAssetsWhenThereIsABetterAssetInTheCarAssetsList(carAssets, possibleCarAssets);

            AddRemainingPossibleCarAssets(carAssets, possibleCarAssets);
        }

        private static void RemovePossibleCarAssetsWhenThereIsABetterAssetInTheCarAssetsList(List<AssetSetAsset> carAssets, Dictionary<string, IList<AssetSetAsset>> possibleCarAssets)
        {
            throw new NotImplementedException();
        }

        private static void AddRemainingPossibleCarAssets(List<AssetSetAsset> carAssets, Dictionary<string, IList<AssetSetAsset>> possibleCarAssets)
        {
            throw new NotImplementedException();
        }

        private void FillAssets(ModelGeneration modelGeneration, ContextData contextData)
        {
            contextData.Assets =
                FillBodyTypeAssets(modelGeneration)
                .Concat(FillEngineAssets(modelGeneration))
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value);
        }

        private Dictionary<Guid, List<Asset>> FillBodyTypeAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.BodyTypes.ToDictionary(
                bodytype => bodytype.ID,
                bodytype =>
                    bodytype.AssetSet.Assets.GetGenerationAssets()
                        .Select(asset => _assetMapper.MapAssetSetAsset(asset, modelGeneration)).ToList());
        }

        private Dictionary<Guid, List<Asset>> FillEngineAssets(ModelGeneration modelGeneration)
        {
            return modelGeneration.Engines.ToDictionary(
                engine => engine.ID,
                engine => engine.AssetSet.Assets
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
                contextData.Cars.Add(_carMapper.MapCar(car, bodyType, engine, transmission));
                contextData.CarAssets.Add(car.ID, new List<Asset>());
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

            Func<Administration.Car, Car> MapCar = dbCar => cars.Single(car => car.ID == dbCar.ID);

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
