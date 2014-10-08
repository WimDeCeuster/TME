using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Enums;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.Carconfigurator.Tests.Builders
{
    public class ContextBuilder
    {
        readonly IContext _context;
        String[] _languages;

        private ContextBuilder(IContext context)
        {
            _context = context;
        }
 
        public static ContextBuilder InitialiseFakeContext()
        {
            var context = A.Fake<IContext>();

            A.CallTo(() => context.Brand).Returns("not initialised");
            A.CallTo(() => context.Country).Returns("not initialized");
            A.CallTo(() => context.ContextData).Returns(new Dictionary<String, ContextData>());
            A.CallTo(() => context.TimeFrames).Returns(new Dictionary<String, IReadOnlyList<TimeFrame>>());

            return new ContextBuilder(context);
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

        public ContextBuilder WithGenerationBodyTypes(String language, params BodyType[] bodyTypes)
        {
            foreach (var bodyType in bodyTypes)
                _context.ContextData[language].GenerationBodyTypes.Add(bodyType);
            return this;
        }

        public ContextBuilder WithGenerationEngines(String language, params Engine[] engines)
        {
            foreach (var engine in engines)
                _context.ContextData[language].GenerationEngines.Add(engine);
            return this;
        }

        private ContextBuilder WithTimeFrames(String language, IEnumerable<TimeFrame> timeFrames)
        {
            _context.TimeFrames[language] = timeFrames.ToList();
            return this;
        }

        private void WithModels(String language, Model model)
        {
            _context.ContextData[language].Models.Add(model);
        }

        public IContext Build()
        {
            return _context;
        }

        public static Generation CreateFakeGeneration(String language)
        {
            return FillFakeBaseObject(new Generation
            {
                CarConfiguratorVersion = new CarConfiguratorVersion
                {
                    Name = "carConfigVersion-" + language
                },
                SSN =  "SSN1-" + language,
                Assets = new List<Asset>
                {
                    CreateFakeAsset("Asset1"),
                    CreateFakeAsset("Asset2"),
                    CreateFakeAsset("Asset3")
                }
            }, "", language);
        }

        public static Car CreateFakeCar(String language, String name)
        {
            return FillFakeBaseObject(new Car
            {
                ID = Guid.NewGuid()
            }, name, language);
        }

        private static Asset CreateFakeAsset(string name)
        {
            return new Asset() {ID = Guid.NewGuid(), Name = name};
        }

        public static T FillFakeBaseObject<T>(T baseObject, String name, String language) where T : BaseObject
        {
            var objectName = baseObject.GetType().Name;

            baseObject.Description = name + objectName + "Description-" + language;
            baseObject.FootNote = name + objectName + "generationFootNote-" + language;
            baseObject.InternalCode = name + objectName + "generationInternalCode-" + language;
            baseObject.LocalCode = name + objectName + "generationLocalCode-" + language;
            baseObject.Name = name + objectName + "generationName-" + language;
            baseObject.ToolTip = name + objectName + "generationToolTip-" + language;
            baseObject.Labels = new List<Label>
            {
                new Label {
                    Code = objectName + "Label1Code-" + language,
                    Value = objectName + "Label1Value-" + language
                },
                new Label {
                    Code = objectName + "Label2Code-" + language,
                    Value = objectName + "Label2Value-" + language
                }
            };

            return baseObject;
        }

        public static IContext GetDefaultContext(IEnumerable<String> languages)
        {
            var builder = ContextBuilder.InitialiseFakeContext()
                            .WithBrand("Toyota")
                            .WithCountry("DE")
                            .WithDataSubset(PublicationDataSubset.Live)
                            .WithLanguages(languages.ToArray());

            foreach (var language in languages)
            {
                builder.WithGeneration(language, CreateFakeGeneration(language));
                var cars = new [] {
                    CreateFakeCar(language, "Car1"),
                    CreateFakeCar(language, "Car2"),
                    CreateFakeCar(language, "Car3")
                };

                builder.WithCars(language, cars);

                var timeFrames = new[] {
                    new TimeFrame(new DateTime(2014, 1, 1), 
                                    new DateTime(2014, 4, 4),
                                    cars.Take(1).ToList()),

                    new TimeFrame(new DateTime(2014, 4, 4), 
                                    new DateTime(2014, 8, 8),
                                    cars.Take(2).ToList()),

                    new TimeFrame(new DateTime(2014, 8, 8), 
                                    new DateTime(2014, 12, 12),
                                    cars.Skip(1).Take(2).ToList())
                };

                builder.WithTimeFrames(language, timeFrames);
                builder.WithModels(language,new Model());
            }

            return builder.Build();
        }
    }
}
