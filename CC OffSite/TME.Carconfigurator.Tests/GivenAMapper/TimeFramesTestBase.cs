using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using FakeItEasy;
using TME.CarConfigurator.Publisher.Enums;

namespace TME.Carconfigurator.Tests.GivenAMapper
{
    public abstract class TimeFramesTestBase : TestBase
    {
        protected Mapper _mapper;
        protected String _brand;
        protected String _country;
        protected String _language;
        protected Int32 _minimumCarCount;
        protected ModelGeneration _generation;
        protected ICarDbModelGenerationFinder _generationFinder;
        protected IContext _context;
        protected Model _model;

        protected override void Arrange()
        {
            AutoMapperConfig.Configure();

            _brand = "Toyota";

            MyContext.SetSystemContext("Toyota", "ZZ", "en");

            var generationInfo = SetupTimeFrameData(out _country, out _language, out _minimumCarCount);
            _generation = generationInfo.Item1;
            _model = generationInfo.Item2;

            _generationFinder = A.Fake<ICarDbModelGenerationFinder>();
            _mapper = new Mapper();

            A.CallTo(() => _generationFinder.GetModelGeneration(_brand, _country, _generation.ID)).Returns(new Dictionary<String, Tuple<ModelGeneration, Model>> { { _language, Tuple.Create(_generation, _model) } });
        }

        Tuple<ModelGeneration, Model> SetupTimeFrameData(out String countryCode, out String languageCode, out Int32 minimumCarCount)
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

            var generationInfo = FindCompatibleModelGeneration(minimumCarCount, out countryCode, out languageCode);
            var cars = generationInfo.Item1.Cars;

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

            return generationInfo;
        }

        Tuple<ModelGeneration, Model> FindCompatibleModelGeneration(Int32 minimumCarCount, out String countryCode, out String languageCode)
        {
            foreach (var country in MyContext.GetContext().Countries)
            {
                countryCode = country.Code;
                languageCode = country.Languages.First().Code;
                MyContext.SetSystemContext(_brand, countryCode, languageCode);
                foreach (var model in Models.GetModels())
                    foreach (var generation in model.Generations)
                        if (generation.Cars.Count >= minimumCarCount)
                            return Tuple.Create(generation, model);
            }

            throw new Exception(String.Format("No generation with at least {0} cars found.", minimumCarCount));
        }
    }
}
