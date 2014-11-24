using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Packs;
using TME.CarConfigurator.Repository.Objects.TechnicalSpecifications;


namespace TME.Carconfigurator.Tests.Builders
{
    public class ContextBuilder
    {
        readonly IContext _context;
        String[] _languages;
 
        public ContextBuilder()
        {
            _context = A.Fake<IContext>();
            A.CallTo(() => _context.Brand).Returns("not initialised");
            A.CallTo(() => _context.Country).Returns("not initialized");
            A.CallTo(() => _context.ContextData).Returns(new Dictionary<String, ContextData>());
            A.CallTo(() => _context.TimeFrames).Returns(new Dictionary<String, IReadOnlyList<TimeFrame>>());
        }

        public ContextBuilder WithBrand(String brand)
        {
            A.CallTo(() => _context.Brand).Returns(brand);
            return this;
        }

        public ContextBuilder WithCountry(String country)
        {
            A.CallTo(() => _context.Country).Returns(country);
            return this;
        }

        public ContextBuilder WithDataSubset(PublicationDataSubset dataSubset)
        {
            A.CallTo(() => _context.DataSubset).Returns(dataSubset);
            return this;
        }

        public ContextBuilder WithLanguages(params String[] languages)
        {
            _languages = languages.ToArray();

            foreach (var language in _languages)
            {
                _context.ContextData[language] = new ContextData();
                _context.TimeFrames[language] = new TimeFrame[] { };
                _context.ModelGenerations[language] = null;
            }

            return this;
        }

        public ContextBuilder WithPublication(String language, Publication publication)
        {
            _context.ContextData[language].Publication = publication;
            return this;
        }

        public ContextBuilder WithGeneration(String language, Generation generation)
        {
            _context.ContextData[language].Generations.Add(generation);
            return this;
        }

        public ContextBuilder WithTimeFrames(String language, params TimeFrame[] timeFrames)
        {
            _context.TimeFrames[language] = timeFrames;
            return this;
        }

        public ContextBuilder WithAssets(String language, List<Asset> assets, Guid objectID)
        {
                _context.ContextData[language].Assets.Add(objectID,assets);    
            return this;
        }

        public ContextBuilder WithModel(String language, Model model)
        {
            _context.ContextData[language].Models.Add(model);

            return this;
        }

        public ContextBuilder AddCarAsset(string language, Guid carId, Guid objectId, Asset asset)
        {
            var data = _context.ContextData[language];
            
            var carAssets = data.CarAssets;

            if (!carAssets.ContainsKey(carId))
                carAssets.Add(carId, new Dictionary<Guid, IList<Asset>>());

            var objectAssets = carAssets[carId];

            if (! objectAssets.ContainsKey(objectId))
                objectAssets.Add(objectId, new List<Asset>());

            var assets = objectAssets[objectId];

            assets.Add(asset);

            return this;
        }

        public ContextBuilder WithCarEquipment(String language, Guid carID, CarEquipment carEquipment)
        {
            var data = _context.ContextData[language];
            var carCarEquipment = data.CarEquipment;

            if (!carCarEquipment.ContainsKey(carID))
                carCarEquipment.Add(carID, carEquipment);
            else
                carCarEquipment[carID] = carEquipment;

            return this;
        }

        public ContextBuilder AddSubModelAsset(string language, Guid subModelID, Guid objectId, Asset asset)
        {
            var data = _context.ContextData[language];

            var subModelAssets = data.SubModelAssets;

            if(!subModelAssets.ContainsKey(subModelID))
                subModelAssets.Add(subModelID,new Dictionary<Guid, IList<Asset>>());

            var objectAssets = subModelAssets[subModelID];

            if (!objectAssets.ContainsKey(objectId))
                objectAssets.Add(objectId,new List<Asset>());

            var assets = objectAssets[objectId];

            assets.Add(asset);

            return this;
        }

        public ContextBuilder WithCarParts(string language, Guid carID, CarPart[] carParts)
        {
            var data = _context.ContextData[language];
            var carCarParts = data.CarParts;

            if (!carCarParts.ContainsKey(carID))
                carCarParts.Add(carID, new List<CarPart>());

            foreach (var carPart in carParts)
            {
                carCarParts[carID].Add(carPart);    
            }

            return this;
        }

        public ContextBuilder WithCarPacks(string language, Guid carId, params CarPack[] packs)
        {
            var data = _context.ContextData[language];
            var carPacks = data.CarPacks;

            carPacks.Add(carId, packs.ToList());

            return this;
        }
        
        public ContextBuilder WithCarSpecs(string language, Guid carId, params CarTechnicalSpecification[] specs)
        {
            var data = _context.ContextData[language];
            var carspecs = data.CarTechnicalSpecifications;

            carspecs.Add(carId, specs.ToList());

            return this;
        }

        public ContextBuilder WithEquipmentCategories(string language, params CarConfigurator.Repository.Objects.Equipment.Category[] equipmentCategories)
        {
            _context.ContextData[language].EquipmentCategories = equipmentCategories.ToList();

            return this;
        }

        public ContextBuilder WithSpecificationCategories(string language, params CarConfigurator.Repository.Objects.TechnicalSpecifications.Category[] specificationCategories)
        {
            _context.ContextData[language].SpecificationCategories = specificationCategories.ToList();

            return this;
        }

        public ContextBuilder AddCarColourCombination(String language, Guid carID, params CarColourCombination[] colourCombinations)
        {
            var data = _context.ContextData[language];
            var carColourCombinations = data.CarColourCombinations;

            if (!carColourCombinations.ContainsKey(carID))
                carColourCombinations.Add(carID, new List<CarColourCombination>());

            foreach (var colourCombination in colourCombinations)
                carColourCombinations[carID].Add(colourCombination);

            return this;
        }

        public ContextBuilder AddCarEquipmentAssets(string language, Guid carId, Guid objectId, params Asset[] assets)
        {
            var data = _context.ContextData[language];
            var carEquipmentAssets = data.CarEquipmentAssets;

            if (!carEquipmentAssets.ContainsKey(carId))
                carEquipmentAssets.Add(carId,new Dictionary<Guid, IList<Asset>>());

            carEquipmentAssets[carId].Add(objectId,new List<Asset>(assets));
            return this;
        }

        public ContextBuilder AddCarPartAssets(string language, Guid carId, Guid objectId, params Asset[] assets)
        {
            var data = _context.ContextData[language];
            var carPartAssets = data.CarPartAssets;

            if (!carPartAssets.ContainsKey(carId))
                carPartAssets.Add(carId,new Dictionary<Guid, IList<Asset>>());

            carPartAssets[carId].Add(objectId,new List<Asset>(assets));
            return this;
        }

        public IContext Build()
        {
            return _context;
        }
    }
}
