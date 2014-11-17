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

namespace TME.Carconfigurator.Tests.GivenAS3WheelDrivePublisher
{
    public class WhenPublishingGenerationWheelDrives : TestBase
    {
        const String _brand = "Toyota";
        const String _country = "DE";
        const String _serialisedWheelDrive1 = "serialised wheelDrive 1";
        const String _serialisedWheelDrive12 = "serialised wheelDrive 1+2";
        const String _serialisedWheelDrive34 = "serialised wheelDrive 3+4";
        const String _serialisedWheelDrive4 = "serialised wheelDrive 4";
        const String _language1 = "lang 1";
        const String _language2 = "lang 2";
        const String _timeFrame1WheelDrivesKey = "time frame 1 wheelDrives key";
        const String _timeFrame2WheelDrivesKey = "time frame 2 wheelDrives key";
        const String _timeFrame3WheelDrivesKey = "time frame 3 wheelDrives key";
        const String _timeFrame4WheelDrivesKey = "time frame 4 wheelDrives key";
        IService _s3Service;
        IWheelDriveService _service;
        IWheelDrivePublisher _publisher;
        IContext _context;

        protected override void Arrange()
        {
            var wheelDrive1 = new WheelDrive { ID = Guid.NewGuid() };
            var wheelDrive2 = new WheelDrive { ID = Guid.NewGuid() };
            var wheelDrive3 = new WheelDrive { ID = Guid.NewGuid() };
            var wheelDrive4 = new WheelDrive { ID = Guid.NewGuid() };

            var timeFrame1 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithWheelDrives(new[] { wheelDrive1 })
                                .Build();
            var timeFrame2 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithWheelDrives(new[] { wheelDrive1, wheelDrive2 })
                                .Build();
            var timeFrame3 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithWheelDrives(new[] { wheelDrive3, wheelDrive4 })
                                .Build();
            var timeFrame4 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithWheelDrives(new[] { wheelDrive4 })
                                .Build();
            
            var publicationTimeFrame1 = new PublicationTimeFrame { ID = timeFrame1.ID };
            var publicationTimeFrame2 = new PublicationTimeFrame { ID = timeFrame2.ID };
            var publicationTimeFrame3 = new PublicationTimeFrame { ID = timeFrame3.ID };
            var publicationTimeFrame4 = new PublicationTimeFrame { ID = timeFrame4.ID };

            var publication1 = new PublicationBuilder()
                                .WithTimeFrames(publicationTimeFrame1,
                                                publicationTimeFrame2)
                                .Build();

            var publication2 = new PublicationBuilder()
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

            _service = new WheelDriveService(_s3Service, serialiser, keyManager);
            _publisher = new WheelDrivePublisherBuilder().WithService(_service).Build();


            A.CallTo(() => serialiser.Serialise((A<IEnumerable<WheelDrive>>.That.IsSameSequenceAs(new[] { wheelDrive1 }))))
                .Returns(_serialisedWheelDrive1);
            A.CallTo(() => serialiser.Serialise((A<IEnumerable<WheelDrive>>.That.IsSameSequenceAs(new[] { wheelDrive1, wheelDrive2 }))))
                .Returns(_serialisedWheelDrive12);
            A.CallTo(() => serialiser.Serialise((A<IEnumerable<WheelDrive>>.That.IsSameSequenceAs(new[] { wheelDrive3, wheelDrive4 }))))
                .Returns(_serialisedWheelDrive34);
            A.CallTo(() => serialiser.Serialise((A<IEnumerable<WheelDrive>>.That.IsSameSequenceAs(new[] { wheelDrive4 }))))
                .Returns(_serialisedWheelDrive4);

            A.CallTo(() => keyManager.GetWheelDrivesKey(publication1.ID, publicationTimeFrame1.ID)).Returns(_timeFrame1WheelDrivesKey);
            A.CallTo(() => keyManager.GetWheelDrivesKey(publication1.ID, publicationTimeFrame2.ID)).Returns(_timeFrame2WheelDrivesKey);
            A.CallTo(() => keyManager.GetWheelDrivesKey(publication2.ID, publicationTimeFrame3.ID)).Returns(_timeFrame3WheelDrivesKey);
            A.CallTo(() => keyManager.GetWheelDrivesKey(publication2.ID, publicationTimeFrame4.ID)).Returns(_timeFrame4WheelDrivesKey);
        }

        protected override void Act()
        {
             _publisher.PublishGenerationWheelDrivesAsync(_context).Wait();
        }

        [Fact]
        public void ThenGenerationWheelDrivesShouldBePutForAllLanguagesAndTimeFrames()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustHaveHappened(Repeated.Exactly.Times(4));
        }

        [Fact]
        public void ThenGenerationWheelDrivesShouldBePutForTimeFrame1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame1WheelDrivesKey, _serialisedWheelDrive1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationWheelDrivesShouldBePutForTimeFrame2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame2WheelDrivesKey, _serialisedWheelDrive12))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationWheelDrivesShouldBePutForTimeFrame3()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame3WheelDrivesKey, _serialisedWheelDrive34))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationWheelDrivesShouldBePutForTimeFrame4()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame4WheelDrivesKey, _serialisedWheelDrive4))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
