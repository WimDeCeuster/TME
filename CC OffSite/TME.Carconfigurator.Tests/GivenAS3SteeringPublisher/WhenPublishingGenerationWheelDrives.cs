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
            var steering1 = new Steering { ID = Guid.NewGuid() };
            var steering2 = new Steering { ID = Guid.NewGuid() };
            var steering3 = new Steering { ID = Guid.NewGuid() };
            var steering4 = new Steering { ID = Guid.NewGuid() };

            var timeFrame1 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithSteerings(new[] { steering1 })
                                .Build();
            var timeFrame2 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithSteerings(new[] { steering1, steering2 })
                                .Build();
            var timeFrame3 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithSteerings(new[] { steering3, steering4 })
                                .Build();
            var timeFrame4 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithSteerings(new[] { steering4 })
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

            _service = new SteeringService(_s3Service, serialiser, keyManager);
            _publisher = new SteeringPublisherBuilder().WithService(_service).Build();

            A.CallTo(() => serialiser.Serialise((IEnumerable<Steering>)null))
                .WhenArgumentsMatch(ArgumentMatchesList(steering1))
                .Returns(_serialisedSteering1);
            A.CallTo(() => serialiser.Serialise((IEnumerable<Steering>)null))
                .WhenArgumentsMatch(ArgumentMatchesList(steering1, steering2))
                .Returns(_serialisedSteering12);
            A.CallTo(() => serialiser.Serialise((IEnumerable<Steering>)null))
                .WhenArgumentsMatch(ArgumentMatchesList(steering3, steering4))
                .Returns(_serialisedSteering34);
            A.CallTo(() => serialiser.Serialise((IEnumerable<Steering>)null))
                .WhenArgumentsMatch(ArgumentMatchesList(steering4))
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
