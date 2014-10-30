using FakeItEasy;
using System;
using System.Collections.Generic;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.GivenAS3CarPublisher
{
    public class WhenPublishingGenerationCars : TestBase
    {
        const String _brand = "Toyota";
        const String _country = "DE";
        const String _serialisedCar1 = "serialised car 1";
        const String _serialisedCar12 = "serialised car 1+2";
        const String _serialisedCar34 = "serialised car 3+4";
        const String _serialisedCar4 = "serialised car 4";
        const String _language1 = "lang 1";
        const String _language2 = "lang 2";
        const String _timeFrame1CarsKey = "time frame 1 car key";
        const String _timeFrame2CarsKey = "time frame 2 car key";
        const String _timeFrame3CarsKey = "time frame 3 car key";
        const String _timeFrame4CarsKey = "time frame 4 car key";
        IService _s3Service;
        ICarService _service;
        ICarPublisher _publisher;
        IContext _context;

        protected override void Arrange()
        {
            var car1 = new Car { ID = Guid.NewGuid() };
            var car2 = new Car { ID = Guid.NewGuid() };
            var car3 = new Car { ID = Guid.NewGuid() };
            var car4 = new Car { ID = Guid.NewGuid() };

            var timeFrame1 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithCars(new[] { car1 })
                                .Build();
            var timeFrame2 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithCars(new[] { car1, car2 })
                                .Build();
            var timeFrame3 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithCars(new[] { car3, car4 })
                                .Build();
            var timeFrame4 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithCars(new[] { car4 })
                                .Build();
            
            var publicationTimeFrame1 = new PublicationTimeFrame { ID = timeFrame1.ID };
            var publicationTimeFrame2 = new PublicationTimeFrame { ID = timeFrame2.ID };
            var publicationTimeFrame3 = new PublicationTimeFrame { ID = timeFrame3.ID };
            var publicationTimeFrame4 = new PublicationTimeFrame { ID = timeFrame4.ID };

            var publication1 = PublicationBuilder.Initialize()
                                                 .WithTimeFrames(publicationTimeFrame1,
                                                                 publicationTimeFrame2)
                                                 .Build();

            var publication2 = PublicationBuilder.Initialize()
                                                 .WithTimeFrames(publicationTimeFrame3,
                                                                 publicationTimeFrame4)
                                                 .Build();

            _context = new ContextBuilder()
                        .WithBrand(_brand)
                        .WithCountry(_country)
                        .WithLanguages(_language1, _language2)
                        .WithPublication(_language1, publication1)
                        .WithPublication(_language2, publication2)
                        .WithTimeFrames(_language1, timeFrame1, timeFrame2)
                        .WithTimeFrames(_language2, timeFrame3, timeFrame4)
                        .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            var keyManager = A.Fake<IKeyManager>();

            _service = new CarServiceBuilder()
                .WithKeyManager(keyManager)
                .WithSerialiser(serialiser)
                .WithService(_s3Service)
                .Build();

            _publisher = new CarPublisherBuilder()
                .WithService(_service)
                .Build();

            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Car>>.That.IsSameSequenceAs(new List<Car> { car1 })))
                .Returns(_serialisedCar1);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Car>>.That.IsSameSequenceAs(new List<Car> { car1, car2 })))
                .Returns(_serialisedCar12);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Car>>.That.IsSameSequenceAs(new List<Car> { car3, car4 })))
                .Returns(_serialisedCar34);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Car>>.That.IsSameSequenceAs(new List<Car> { car4 })))
                .Returns(_serialisedCar4);

            A.CallTo(() => keyManager.GetCarsKey(publication1.ID, publicationTimeFrame1.ID)).Returns(_timeFrame1CarsKey);
            A.CallTo(() => keyManager.GetCarsKey(publication1.ID, publicationTimeFrame2.ID)).Returns(_timeFrame2CarsKey);
            A.CallTo(() => keyManager.GetCarsKey(publication2.ID, publicationTimeFrame3.ID)).Returns(_timeFrame3CarsKey);
            A.CallTo(() => keyManager.GetCarsKey(publication2.ID, publicationTimeFrame4.ID)).Returns(_timeFrame4CarsKey);
        }

        protected override void Act()
        {
            var result = _publisher.PublishGenerationCarsAsync(_context).Result;
        }

        [Fact]
        public void ThenGenerationCarsShouldBePutForAllLanguagesAndTimeFrames()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustHaveHappened(Repeated.Exactly.Times(4));
        }

        [Fact]
        public void ThenGenerationCarsShouldBePutForTimeFrame1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame1CarsKey, _serialisedCar1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationCarsShouldBePutForTimeFrame2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame2CarsKey, _serialisedCar12))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationCarsShouldBePutForTimeFrame3()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame3CarsKey, _serialisedCar34))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationCarsShouldBePutForTimeFrame4()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame4CarsKey, _serialisedCar4))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
