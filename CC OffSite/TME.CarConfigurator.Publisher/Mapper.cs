﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Assets;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
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
                FillWheelDrives(modelGeneration, contextData);

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
            return (asset.IsDeviation() || !asset.AlwaysInclude)
                   && (asset.BodyType.IsEmpty() || asset.BodyType.Equals(car.BodyType))
                   && (asset.Engine.IsEmpty() || asset.Engine.Equals(car.Engine))
                   && (asset.Transmission.IsEmpty() || asset.Transmission.Equals(car.Transmission))
                   && (asset.WheelDrive.IsEmpty() || asset.WheelDrive.Equals(car.WheelDrive))
                   && (asset.Steering.IsEmpty() || asset.Steering.Equals(car.Steering))
                   && (asset.Grade.IsEmpty() || asset.Grade.Equals(car.Grade));
        }

        private static bool ThereIsAnotherAssetThatFitsEquallyWellButItAlsoFitsOnTheGradeOfTheCar(Administration.Car car, AssetSetAsset asset, IEnumerable<AssetSetAsset> assets)
        {
            if (asset.Grade.ID == car.GradeID) return false; // There is no asset that is the same with a better grade than the car, because this asset already has the same grade. There can (potentially/theoretically) be other assets with the exact same deviation properties, but those shouldn't stop this one from being added. 

            return assets.Any(otherAsset => IsAssetOfCar(car, otherAsset)
                && otherAsset.AssetType.Equals(asset.AssetType)
                && (asset.BodyType.IsEmpty() || otherAsset.BodyType.Equals(asset.BodyType)) // Only check if bodytype is the same if the current asset has a bodytype, otherwise it can be ignored
                && otherAsset.Engine.Equals(asset.Engine)
                && otherAsset.Transmission.Equals(asset.Transmission)
                && otherAsset.WheelDrive.Equals(asset.WheelDrive)
                && otherAsset.Steering.Equals(asset.Steering)
                && otherAsset.ExteriorColour.Equals(asset.ExteriorColour)
                && otherAsset.Upholstery.Equals(asset.Upholstery)
                && otherAsset.EquipmentItem.Equals(asset.EquipmentItem)
                && otherAsset.Grade.Equals(car.Grade)); // if it has all other properties the same, and in addition has the same grade as the car, it is the asset we are looking for
        }

        private static void TryAddAssetToPossibleCarAssets(AssetSetAsset asset, Dictionary<string, IList<AssetSetAsset>> possibleCarAssets)
        {
            var key = GetKey(asset);

            if (!possibleCarAssets.ContainsKey(key))
            {
                possibleCarAssets.Add(key, new List<AssetSetAsset> { asset });
                return;
            }

            var existingAssets = possibleCarAssets[key];

            var existingAsset = existingAssets.First(); // all assets for this key the same number of deviation axes

            var numberOfDeviationAxesOfExistingAsset = GetNumberOfDeviationAxes(existingAsset);
            var numberOfDeviationAxesOfCurrentAsset = GetNumberOfDeviationAxes(asset);

            if (numberOfDeviationAxesOfCurrentAsset < numberOfDeviationAxesOfExistingAsset)
                return;

            if (numberOfDeviationAxesOfCurrentAsset == numberOfDeviationAxesOfExistingAsset)
            {
                existingAssets.Add(asset);

                return;
            }

            // more deviation axes than existing assets => only keep the current asset
            possibleCarAssets[key] = new List<AssetSetAsset> { asset };
        }

        private static string GetKey(AssetSetAsset asset)
        {
            var assetType = asset.AssetType;

            var mode = string.Empty;
            var view = string.Empty;
            var side = string.Empty;
            var type = string.Empty;


            var splitAssetTypeName = assetType.Name.Split('_');
            var sections = splitAssetTypeName.Length;

            if (!string.IsNullOrEmpty(assetType.Details.Mode))
            {
                const int leastExpectedNumberOfSections = 3;
                if (sections < leastExpectedNumberOfSections)
                    throw new CorruptDataException(string.Format("At least 4 parts should be definied in the type {0} for the {1} mode", assetType.Name, assetType.Details.Mode));

                if (sections > leastExpectedNumberOfSections)
                {
                    mode = splitAssetTypeName[0];
                    view = splitAssetTypeName[1];
                    side = splitAssetTypeName[2];
                    type = splitAssetTypeName[3];
                }
            }
            else if (sections > 2)
            {
                view = splitAssetTypeName[0];
                side = splitAssetTypeName[1];
                type = splitAssetTypeName[2];
            }

            if (string.IsNullOrEmpty(view))
                return assetType.Code;

            var exteriourColourCode = asset.ExteriorColour.Code;
            var upholsteryCode = asset.Upholstery.Code;
            var equipmentCode = asset.EquipmentItem.ID.ToString(); // TODO: replace this by equipment item code, either by adding to equipmentinfo object or by fetching from ModelGeneration => Check with Wim

            return GetKey(mode, view, side, type, exteriourColourCode, upholsteryCode, equipmentCode);
        }

        private static string GetKey(string mode, string view, string side, string type, string exteriourColourCode, string upholsteryCode, string equipmentCode)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(mode);

            if (string.IsNullOrEmpty(view)) 
                return stringBuilder.ToString();

            if (stringBuilder.Length != 0)
                stringBuilder.Append("_"); // if a mode was appended before, we need the underscore

            stringBuilder.Append(view);

            if (string.IsNullOrEmpty(side))
                return stringBuilder.ToString();

            stringBuilder.Append(string.Format("_{0}", side));

            if (string.IsNullOrEmpty(type))
                return stringBuilder.ToString();

            stringBuilder.Append(string.Format("_{0}", type));

            if (string.IsNullOrEmpty(exteriourColourCode))
                return stringBuilder.ToString();

            stringBuilder.Append(string.Format("_{0}", exteriourColourCode));

            if (string.IsNullOrEmpty(upholsteryCode))
                return stringBuilder.ToString();

            stringBuilder.Append(string.Format("_{0}", upholsteryCode));

            if (string.IsNullOrEmpty(equipmentCode))
                return stringBuilder.ToString();

            stringBuilder.Append(string.Format("_{0}", equipmentCode));

            return stringBuilder.ToString();
        }

        private static int GetNumberOfDeviationAxes(AssetSetAsset asset)
        {
            var i = 0;

            i += asset.BodyType.IsEmpty() ? 0 : 1;
            i += asset.Engine.IsEmpty() ? 0 : 1;
            i += asset.Grade.IsEmpty() ? 0 : 1;
            i += asset.Transmission.IsEmpty() ? 0 : 1;
            i += asset.WheelDrive.IsEmpty() ? 0 : 1;
            i += asset.Steering.IsEmpty() ? 0 : 1;

            return i;
        }

        private static void AddPossibleCarAssetsThatAreBetterMatches(List<AssetSetAsset> carAssets, Dictionary<string, IList<AssetSetAsset>> possibleCarAssets)
        {
            RemovePossibleCarAssetsWhenThereIsABetterAssetInTheCarAssetsList(carAssets, possibleCarAssets);

            var remainingPossibleCarAssets = possibleCarAssets.Values.SelectMany(v => v);

            carAssets.AddRange(remainingPossibleCarAssets);
        }

        private static void RemovePossibleCarAssetsWhenThereIsABetterAssetInTheCarAssetsList(IEnumerable<AssetSetAsset> carAssets, Dictionary<string, IList<AssetSetAsset>> possibleCarAssets)
        {
            foreach (var carAsset in carAssets)
            {
                var key = GetKey(carAsset);

                if (!possibleCarAssets.ContainsKey(key))
                    continue;

                var existingAsset = possibleCarAssets[key].First(); // all assets for this key have the same number of deviation axes

                var numberOfDeviationAxesOfPossibleAsset = GetNumberOfDeviationAxes(existingAsset);
                var numberOfDeviationAxesOfCarAsset = GetNumberOfDeviationAxes(carAsset);

                if (numberOfDeviationAxesOfPossibleAsset >= numberOfDeviationAxesOfCarAsset) // optional assets are more specific than the car asset, so we can keep them
                    continue;

                possibleCarAssets.Remove(key);
            }
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
                var wheelDrive = contextData.WheelDrives.Single(drive => drive.ID == car.WheelDriveID);
                contextData.Cars.Add(_carMapper.MapCar(car, bodyType, engine, transmission, wheelDrive));
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
