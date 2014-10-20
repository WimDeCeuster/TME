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
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Publisher;

namespace TME.Carconfigurator.Tests.GivenAS3SteeringPublisher
{
    public class WhenPublishingGenerationSteerings : TestBase
    {
        const String _brand = "Toyota";
        const String _country = "DE";
        const String _serialisedSteering1 = "serialised steering 1";
        const String _serialisedSteering12 = "serialised steering 1+2";
        const String _serialisedSteering34 = "serialised steering 3+4";
        const String _serialisedSteering4 = "serialised steering 4";
        const String _language1 = "lang 1";
        const String _language2 = "lang 2";
        const String _timeFrame1SteeringsKey = "time frame 1 steerings key";
        const String _timeFrame2SteeringsKey = "time frame 2 steerings key";
        const String _timeFrame3SteeringsKey = "time frame 3 steerings key";
        const String _timeFrame4SteeringsKey = "time frame 4 steerings key";
        IService _s3Service;
        ISteeringService _service;
        ISteeringPublisher _publisher;
        IContext _context;

        protected override void Arrange()
        {
            var steeringId1 = Guid.NewGuid();
            var steeringId2 = Guid.NewGuid();
            var steeringId3 = Guid.NewGuid();
            var steeringId4 = Guid.NewGuid();

            var car1 = new Car { Steering = new Steering { ID = steeringId1 } };
            var car2 = new Car { Steering = new Steering { ID = steeringId2 } };
            var car3 = new Car { Steering = new Steering { ID = steeringId3 } };
            var car4 = new Car { Steering = new Steering { ID = steeringId4 } };

            var timeFrame1 = new TimeFrame(DateTime.MinValue, DateTime.MaxValue, new[] { car1 });
            var timeFrame2 = new TimeFrame(DateTime.MinValue, DateTime.MaxValue, new[] { car1, car2 });
            var timeFrame3 = new TimeFrame(DateTime.MinValue, DateTime.MaxValue, new[] { car3, car4 });
            var timeFrame4 = new TimeFrame(DateTime.MinValue, DateTime.MaxValue, new[] { car4 });

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

            var generationSteering1 = new Steering { ID = steeringId1 };
            var generationSteering2 = new Steering { ID = steeringId2 };
            var generationSteering3 = new Steering { ID = steeringId3 };
            var generationSteering4 = new Steering { ID = steeringId4 };

            _context = new ContextBuilder()
                        .WithBrand(_brand)
                        .WithCountry(_country)
                        .WithLanguages(_language1, _language2)
                        .WithPublication(_language1, publication1)
                        .WithPublication(_language2, publication2)
                        .WithCars(_language1, car1, car2)
                        .WithCars(_language2, car3, car4)
                        .WithTimeFrames(_language1, timeFrame1, timeFrame2)
                        .WithTimeFrames(_language2, timeFrame3, timeFrame4)
                        .WithSteerings(_language1, generationSteering1, generationSteering2)
                        .WithSteerings(_language2, generationSteering3, generationSteering4)
                        .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            var keyManager = A.Fake<IKeyManager>();

            _service = new SteeringService(_s3Service, serialiser, keyManager);
            _publisher = new SteeringPublisher(_service);

            A.CallTo(() => serialiser.Serialise((IEnumerable<Steering>)null))
                .WhenArgumentsMatch(ArgumentMatchesList(generationSteering1))
                .Returns(_serialisedSteering1);
            A.CallTo(() => serialiser.Serialise((IEnumerable<Steering>)null))
                .WhenArgumentsMatch(ArgumentMatchesList(generationSteering1, generationSteering2))
                .Returns(_serialisedSteering12);
            A.CallTo(() => serialiser.Serialise((IEnumerable<Steering>)null))
                .WhenArgumentsMatch(ArgumentMatchesList(generationSteering3, generationSteering4))
                .Returns(_serialisedSteering34);
            A.CallTo(() => serialiser.Serialise((IEnumerable<Steering>)null))
                .WhenArgumentsMatch(ArgumentMatchesList(generationSteering4))
                .Returns(_serialisedSteering4);

            A.CallTo(() => keyManager.GetSteeringsKey(publication1.ID, publicationTimeFrame1.ID)).Returns(_timeFrame1SteeringsKey);
            A.CallTo(() => keyManager.GetSteeringsKey(publication1.ID, publicationTimeFrame2.ID)).Returns(_timeFrame2SteeringsKey);
            A.CallTo(() => keyManager.GetSteeringsKey(publication2.ID, publicationTimeFrame3.ID)).Returns(_timeFrame3SteeringsKey);
            A.CallTo(() => keyManager.GetSteeringsKey(publication2.ID, publicationTimeFrame4.ID)).Returns(_timeFrame4SteeringsKey);
        }

        protected override void Act()
        {
            var result = _publisher.PublishGenerationSteerings(_context).Result;
        }

        [Fact]
        public void ThenGenerationSteeringsShouldBePutForAllLanguagesAndTimeFrames()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(null, null, null, null))
                .WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Times(4));
        }

        [Fact]
        public void ThenGenerationSteeringsShouldBePutForTimeFrame1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame1SteeringsKey, _serialisedSteering1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationSteeringsShouldBePutForTimeFrame2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame2SteeringsKey, _serialisedSteering12))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationSteeringsShouldBePutForTimeFrame3()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame3SteeringsKey, _serialisedSteering34))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationSteeringsShouldBePutForTimeFrame4()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame4SteeringsKey, _serialisedSteering4))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
