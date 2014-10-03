using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Enums;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.Carconfigurator.Tests.Builders
{
    public class ContextBuilder
    {
        Context Context { get; set; }

        public ContextBuilder(String brand, String country, Guid modelGenerationID, PublicationDataSubset dataSubset, IEnumerable<String> languages)
        {
            Context = new Context(brand, country, modelGenerationID, dataSubset);

            foreach (var language in languages)
            {
                Context.ContextData[language] = new ContextData();
                Context.ModelGenerations[language] = null; // A.Fake<TME.CarConfigurator.Administration.ModelGeneration>();
            }
        }
 
        public ContextBuilder WithGeneration(String language, Generation generation)
        {
            Context.ContextData[language].Generations.Add(generation);
            return this;
        }

        public ContextBuilder WithCars(String language, IEnumerable<Car> cars)
        {
            foreach (var car in cars)
                Context.ContextData[language].Cars.Add(car);
            return this;
        }

        private ContextBuilder WithTimeFrames(string language, IEnumerable<TimeFrame> timeFrames)
        {
            Context.TimeFrames[language] = timeFrames.ToList();
            return this;
        }

        private void WithModels(String language, Model model)
        {
            Context.ContextData[language].Models.Add(model);
        }

        public static Generation CreateFakeGeneration(String language)
        {
            return FillFakeBaseObject(new Generation
            {
                CarConfiguratorVersion = new CarConfiguratorVersion
                {
                    Name = "carConfigVersion-" + language
                },
                SSN =  "SSN1-" + language
            }, "", language);
        }

        public static Car CreateFakeCar(String language, String name)
        {
            return FillFakeBaseObject(new Car
            {
                ID = Guid.NewGuid()
            }, name, language);
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

        public static Context GetDefaultContext(IEnumerable<String> languages)
        {
            var builder = new ContextBuilder("Toyota", "BE", Guid.Empty, PublicationDataSubset.Live, languages);

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

            return builder.Context;
        }
    }
}
