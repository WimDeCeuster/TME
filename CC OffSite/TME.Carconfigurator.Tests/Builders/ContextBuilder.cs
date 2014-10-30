using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;


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

        public ContextBuilder WithCars(String language, params Car[] cars)
        {
            foreach (var car in cars)
                _context.ContextData[language].Cars.Add(car);
            return this;
        }

        public ContextBuilder WithTimeFrames(String language, params TimeFrame[] timeFrames)
        {
            _context.TimeFrames[language] = timeFrames;
            return this;
        }

        public ContextBuilder WithTransmissions(String language, params Transmission[] transmissions)
        {
            foreach (var transmission in transmissions)
                _context.ContextData[language].Transmissions.Add(transmission);
            return this;
        }

        public ContextBuilder WithAssets(String language, List<Asset> assets, Guid objectID)
        {
                _context.ContextData[language].Assets.Add(objectID,assets);    
            return this;
        }

        public ContextBuilder WithBodyTypes(String language, List<BodyType> bodyTypes)
        {
            foreach (var bodyType in bodyTypes)
                _context.ContextData[language].BodyTypes.Add(bodyType);
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

        public IContext Build()
        {
            return _context;
        }
    }
}
