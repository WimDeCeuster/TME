using FakeItEasy;
using System;
using System.Collections.Generic;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.S3.CommandServices;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.Publisher;

namespace TME.Carconfigurator.Tests.GivenAS3EnginePublisher
{
    public class WhenPublishingGenerationEngines : TestBase
    {
        const String _brand = "Toyota";
        const String _country = "DE";
        const String _serialisedEngine1 = "serialised engine 1";
        const String _serialisedEngine12 = "serialised engine 1+2";
        const String _serialisedEngine34 = "serialised engine 3+4";
        const String _serialisedEngine4 = "serialised engine 4";
        const String _language1 = "lang 1";
        const String _language2 = "lang 2";
        const String _timeFrame1EnginesKey = "time frame 1 engines key";
        const String _timeFrame2EnginesKey = "time frame 2 engines key";
        const String _timeFrame3EnginesKey = "time frame 3 engines key";
        const String _timeFrame4EnginesKey = "time frame 4 engines key";
        IService _s3Service;
        IEngineService _service;
        IEnginePublisher _publisher;
        IContext _context;

        protected override void Arrange()
        {
            var engineId1 = Guid.NewGuid();
            var engineId2 = Guid.NewGuid();
            var engineId3 = Guid.NewGuid();
            var engineId4 = Guid.NewGuid();

            var car1 = new Car { Engine = new Engine { ID = engineId1 } };
            var car2 = new Car { Engine = new Engine { ID = engineId2 } };
            var car3 = new Car { Engine = new Engine { ID = engineId3 } };
            var car4 = new Car { Engine = new Engine { ID = engineId4 } };

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

            var generationEngine1 = new Engine { ID = engineId1 };
            var generationEngine2 = new Engine { ID = engineId2 };
            var generationEngine3 = new Engine { ID = engineId3 };
            var generationEngine4 = new Engine { ID = engineId4 };

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
                        .WithEngines(_language1, generationEngine1, generationEngine2)
                        .WithEngines(_language2, generationEngine3, generationEngine4)
                        .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            var keyManager = A.Fake<IKeyManager>();

            _service = new EngineService(_s3Service, serialiser, keyManager);
            _publisher = new EnginePublisher(_service);

            A.CallTo(() => serialiser.Serialise((IEnumerable<Engine>)null))
                .WhenArgumentsMatch(ArgumentMatchesList(generationEngine1))
                .Returns(_serialisedEngine1);
            A.CallTo(() => serialiser.Serialise((IEnumerable<Engine>)null))
                .WhenArgumentsMatch(ArgumentMatchesList(generationEngine1, generationEngine2))
                .Returns(_serialisedEngine12);
            A.CallTo(() => serialiser.Serialise((IEnumerable<Engine>)null))
                .WhenArgumentsMatch(ArgumentMatchesList(generationEngine3, generationEngine4))
                .Returns(_serialisedEngine34);
            A.CallTo(() => serialiser.Serialise((IEnumerable<Engine>)null))
                .WhenArgumentsMatch(ArgumentMatchesList(generationEngine4))
                .Returns(_serialisedEngine4);

            A.CallTo(() => keyManager.GetEnginesKey(publication1.ID, publicationTimeFrame1.ID)).Returns(_timeFrame1EnginesKey);
            A.CallTo(() => keyManager.GetEnginesKey(publication1.ID, publicationTimeFrame2.ID)).Returns(_timeFrame2EnginesKey);
            A.CallTo(() => keyManager.GetEnginesKey(publication2.ID, publicationTimeFrame3.ID)).Returns(_timeFrame3EnginesKey);
            A.CallTo(() => keyManager.GetEnginesKey(publication2.ID, publicationTimeFrame4.ID)).Returns(_timeFrame4EnginesKey);
        }

        protected override void Act()
        {
            var result = _publisher.PublishGenerationEngines(_context).Result;
        }

        [Fact]
        public void ThenGenerationEnginesShouldBePutForAllLanguagesAndTimeFrames()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(null, null, null, null))
                .WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Times(4));
        }

        [Fact]
        public void ThenGenerationEnginesShouldBePutForTimeFrame1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame1EnginesKey, _serialisedEngine1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationEnginesShouldBePutForTimeFrame2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame2EnginesKey, _serialisedEngine12))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationEnginesShouldBePutForTimeFrame3()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame3EnginesKey, _serialisedEngine34))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationEnginesShouldBePutForTimeFrame4()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame4EnginesKey, _serialisedEngine4))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
