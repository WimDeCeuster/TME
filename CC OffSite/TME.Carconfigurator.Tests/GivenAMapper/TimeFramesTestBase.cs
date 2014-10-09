using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using FakeItEasy;

namespace TME.Carconfigurator.Tests.GivenAMapper
{
    public abstract class TimeFramesTestBase : TestBase
    {
        protected Mapper mapper;
        protected String brand;
        protected String country;
        protected String language;
        protected Int32 minimumCarCount;
        protected ModelGeneration generation;
        protected ICarDbModelGenerationFinder generationFinder;
        protected IContext context;
        protected Model model;

        protected override void Arrange()
        {
            AutoMapperConfig.Configure();

            brand = "Toyota";

            var timeFrameGenerationInfo = SetupTimeFrameData();
            generation = timeFrameGenerationInfo.ModelGeneration;
            model = timeFrameGenerationInfo.Model;
            country = timeFrameGenerationInfo.Country;
            language = timeFrameGenerationInfo.Language;
            minimumCarCount = timeFrameGenerationInfo.MinimumCarCount;

            generationFinder = A.Fake<ICarDbModelGenerationFinder>();
            mapper = new Mapper();

            A.CallTo(() => generationFinder.GetModelGeneration(brand, country, generation.ID)).Returns(new Dictionary<String, Tuple<ModelGeneration, Model>> { { language, Tuple.Create(generation, model) } });
        }

        TimeFrameGenerationInfo SetupTimeFrameData()
        {
            var timeFramesPre = new[] {
                Tuple.Create(new DateTime(2014, 1, 1), new DateTime(2014, 6, 1)),
                Tuple.Create(new DateTime(2014, 2, 1), new DateTime(2014, 6, 1)),
                Tuple.Create(new DateTime(2014, 2, 1), new DateTime(2014, 6, 1)),
                Tuple.Create(new DateTime(2014, 2, 1), new DateTime(2014, 5, 1)),
                Tuple.Create(new DateTime(2014, 3, 1), new DateTime(2014, 4, 1))
            };

            var timeFramesPost = new[] {
                Tuple.Create(new DateTime(2014, 5, 1), new DateTime(2014, 6, 1)),
                Tuple.Create(new DateTime(2014, 7, 1), new DateTime(2014, 8, 1)),
                Tuple.Create(new DateTime(2014, 9, 1), new DateTime(2014, 11, 1)),
                Tuple.Create(new DateTime(2014, 10, 1), new DateTime(2014, 12, 1))
            };

            minimumCarCount = timeFramesPre.Length + timeFramesPost.Length;

            var timeFrameGenerationInfo = FindCompatibleModelGeneration(minimumCarCount);
            var cars = timeFrameGenerationInfo.ModelGeneration.Cars;

            var extraTimeFrame = Tuple.Create(new DateTime(2014, 3, 10), new DateTime(2014, 3, 20));
            var timeFramesExtra = Enumerable.Repeat(extraTimeFrame, cars.Count - minimumCarCount);

            var timeFrames = timeFramesPre.Concat(timeFramesExtra).Concat(timeFramesPost);

            foreach (var entry in cars.Zip(timeFrames, Tuple.Create))
            {
                var car = entry.Item1;
                var lineOffFrom = entry.Item2.Item1;
                var lineOffTo = entry.Item2.Item2;

                car.LineOffFromDate = lineOffFrom;
                car.LineOffToDate = lineOffTo;
            }

            return timeFrameGenerationInfo;
        }

        TimeFrameGenerationInfo FindCompatibleModelGeneration(Int32 minimumCarCount)
        {
            MyContext.SetSystemContext(brand, "ZZ", "en");
            foreach (var country in MyContext.GetContext().Countries)
            {
                var countryCode = country.Code;
                var languageCode = country.Languages.First().Code;
                MyContext.SetSystemContext(brand, countryCode, languageCode);
                foreach (var model in Models.GetModels())
                    foreach (var generation in model.Generations)
                        if (generation.Cars.Count >= minimumCarCount)
                            return new TimeFrameGenerationInfo
                            {
                                Country = countryCode,
                                Language = languageCode,
                                MinimumCarCount = minimumCarCount,
                                Model = model,
                                ModelGeneration = generation
                            };
            }

            throw new Exception(String.Format("No generation with at least {0} cars found.", minimumCarCount));
        }

        class TimeFrameGenerationInfo
        {
            public String Country { get; set; }
            public String Language { get; set; }
            public Int32 MinimumCarCount { get; set; }
            public ModelGeneration ModelGeneration { get; set; }
            public Model Model { get; set; }
        }
    }
}
