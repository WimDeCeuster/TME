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

namespace TME.Carconfigurator.Tests.GivenAS3TransmissionPublisher
{
    public class WhenPublishingGenerationTransmissions : TestBase
    {
        const String _brand = "Toyota";
        const String _country = "DE";
        const String _serialisedTransmission1 = "serialised transmission 1";
        const String _serialisedTransmission12 = "serialised transmission 1+2";
        const String _serialisedTransmission34 = "serialised transmission 3+4";
        const String _serialisedTransmission4 = "serialised transmission 4";
        const String _language1 = "lang 1";
        const String _language2 = "lang 2";
        const String _timeFrame1TransmissionsKey = "time frame 1 transmissions key";
        const String _timeFrame2TransmissionsKey = "time frame 2 transmissions key";
        const String _timeFrame3TransmissionsKey = "time frame 3 transmissions key";
        const String _timeFrame4TransmissionsKey = "time frame 4 transmissions key";
        IService _s3Service;
        ITransmissionService _service;
        ITransmissionPublisher _publisher;
        IContext _context;

        protected override void Arrange()
        {
            var transmission1 = new Transmission { ID = Guid.NewGuid() };
            var transmission2 = new Transmission { ID = Guid.NewGuid() };
            var transmission3 = new Transmission { ID = Guid.NewGuid() };
            var transmission4 = new Transmission { ID = Guid.NewGuid() };

            var timeFrame1 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithTransmissions(new[] { transmission1 })
                                .Build();
            var timeFrame2 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithTransmissions(new[] { transmission1, transmission2 })
                                .Build();
            var timeFrame3 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithTransmissions(new[] { transmission3, transmission4 })
                                .Build();
            var timeFrame4 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithTransmissions(new[] { transmission4 })
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

            _service = new TransmissionService(_s3Service, serialiser, keyManager);
            _publisher = new TransmissionPublisherBuilder().WithService(_service).Build();

            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Transmission>>.That.IsSameSequenceAs(new[] { transmission1 })))
                .Returns(_serialisedTransmission1);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Transmission>>.That.IsSameSequenceAs(new[] { transmission1, transmission2 })))
                .Returns(_serialisedTransmission12);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Transmission>>.That.IsSameSequenceAs(new[] { transmission3, transmission4 })))
                .Returns(_serialisedTransmission34);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Transmission>>.That.IsSameSequenceAs(new[] { transmission4 })))
                .Returns(_serialisedTransmission4);

            A.CallTo(() => keyManager.GetTransmissionsKey(publication1.ID, publicationTimeFrame1.ID)).Returns(_timeFrame1TransmissionsKey);
            A.CallTo(() => keyManager.GetTransmissionsKey(publication1.ID, publicationTimeFrame2.ID)).Returns(_timeFrame2TransmissionsKey);
            A.CallTo(() => keyManager.GetTransmissionsKey(publication2.ID, publicationTimeFrame3.ID)).Returns(_timeFrame3TransmissionsKey);
            A.CallTo(() => keyManager.GetTransmissionsKey(publication2.ID, publicationTimeFrame4.ID)).Returns(_timeFrame4TransmissionsKey);
        }

        protected override void Act()
        {
            _publisher.PublishGenerationTransmissionsAsync(_context).Wait();
        }

        [Fact]
        public void ThenGenerationTransmissionsShouldBePutForAllLanguagesAndTimeFrames()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustHaveHappened(Repeated.Exactly.Times(4));
        }

        [Fact]
        public void ThenGenerationTransmissionsShouldBePutForTimeFrame1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame1TransmissionsKey, _serialisedTransmission1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationTransmissionsShouldBePutForTimeFrame2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame2TransmissionsKey, _serialisedTransmission12))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationTransmissionsShouldBePutForTimeFrame3()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame3TransmissionsKey, _serialisedTransmission34))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationTransmissionsShouldBePutForTimeFrame4()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame4TransmissionsKey, _serialisedTransmission4))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
