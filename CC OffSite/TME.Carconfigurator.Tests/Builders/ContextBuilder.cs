using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;
using Context = TME.CarConfigurator.Repository.Objects.Context;


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

        public ContextBuilder WithBodyTypes(String language, params BodyType[] bodyTypes)
        {
            foreach (var bodyType in bodyTypes)
                _context.ContextData[language].BodyTypes.Add(bodyType);
            return this;
        }

        public ContextBuilder WithEngines(String language, params Engine[] engines)
        {
            foreach (var engine in engines)
                _context.ContextData[language].Engines.Add(engine);
            return this;
        }

        public ContextBuilder WithTransmissions(String language, params Transmission[] transmissions)
        {
            foreach (var transmission in transmissions)
                _context.ContextData[language].Transmissions.Add(transmission);
            return this;
        }

        public ContextBuilder WithWheelDrives(String language, params WheelDrive[] wheelDrives)
        {
            foreach (var wheelDrive in wheelDrives)
                _context.ContextData[language].WheelDrives.Add(wheelDrive);
            return this;
        }

        public ContextBuilder WithSteerings(String language, params Steering[] steerings)
        {
            foreach (var steering in steerings)
                _context.ContextData[language].Steerings.Add(steering);
            return this;
        }

        public ContextBuilder WithGrades(String language, params Grade[] grades)
        {
            foreach (var grade in grades)
                _context.ContextData[language].Grades.Add(grade);
            return this;
        }

        private ContextBuilder WithTimeFrames(String language, IEnumerable<TimeFrame> timeFrames)
        {
            _context.TimeFrames[language] = timeFrames.ToList();
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

        public IContext Build()
        {
            return _context;
        }
    }
}
