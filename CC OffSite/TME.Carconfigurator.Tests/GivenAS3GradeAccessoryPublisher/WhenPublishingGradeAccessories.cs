using System.Collections.Generic;
using FakeItEasy;
using System;
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
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.Carconfigurator.Tests.GivenAS3GradeAccessoryPublisher
{
    public class WhenPublishingGenerationGradeAccessories : TestBase
    {
        const String _brand = "Toyota";
        const String _country = "DE";
        const String _serialisedGradeAccessory1 = "serialised gradeAccessory 1";
        const String _serialisedGradeAccessory12 = "serialised gradeAccessory 1+2";
        const String _serialisedGradeAccessory34 = "serialised gradeAccessory 3+4";
        const String _serialisedGradeAccessory4 = "serialised gradeAccessory 4";
        const String _language1 = "lang 1";
        const String _language2 = "lang 2";
        const String _timeFrame1GradeAccessoriesKey = "time frame 1 gradeAccessories key";
        const String _timeFrame2GradeAccessoriesKey = "time frame 2 gradeAccessories key";
        const String _timeFrame3GradeAccessoriesKey = "time frame 3 gradeAccessories key";
        const String _timeFrame4GradeAccessoriesKey = "time frame 4 gradeAccessories key";
        IService _s3Service;
        IGradeAccessoryService _service;
        IGradeAccessoryPublisher _publisher;
        IContext _context;

        protected override void Arrange()
        {
            var gradeId = Guid.NewGuid();

            var gradeAccessory1 = new GradeAccessory { ID = Guid.NewGuid() };
            var gradeAccessory2 = new GradeAccessory { ID = Guid.NewGuid() };
            var gradeAccessory3 = new GradeAccessory { ID = Guid.NewGuid() };
            var gradeAccessory4 = new GradeAccessory { ID = Guid.NewGuid() };

            var timeFrame1 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithGradeAccessories(gradeId, new[] { gradeAccessory1 })
                                .Build();
            var timeFrame2 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithGradeAccessories(gradeId, new[] { gradeAccessory1, gradeAccessory2 })
                                .Build();
            var timeFrame3 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithGradeAccessories(gradeId, new[] { gradeAccessory3, gradeAccessory4 })
                                .Build();
            var timeFrame4 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithGradeAccessories(gradeId, new[] { gradeAccessory4 })
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

            _service = new GradeAccessoryService(_s3Service, serialiser, keyManager);
            _publisher = new GradeAccessoryPublisherBuilder().WithService(_service).Build();

            A.CallTo(() => serialiser.Serialise(A<IEnumerable<GradeAccessory>>.That.IsSameSequenceAs(new[] { gradeAccessory1 })))
                .Returns(_serialisedGradeAccessory1);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<GradeAccessory>>.That.IsSameSequenceAs(new[] { gradeAccessory1, gradeAccessory2 })))
                .Returns(_serialisedGradeAccessory12);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<GradeAccessory>>.That.IsSameSequenceAs(new[] { gradeAccessory3, gradeAccessory4 })))
                .Returns(_serialisedGradeAccessory34);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<GradeAccessory>>.That.IsSameSequenceAs(new[] { gradeAccessory4 })))
                .Returns(_serialisedGradeAccessory4);

            A.CallTo(() => keyManager.GetGradeAccessoriesKey(publication1.ID, publicationTimeFrame1.ID, gradeId)).Returns(_timeFrame1GradeAccessoriesKey);
            A.CallTo(() => keyManager.GetGradeAccessoriesKey(publication1.ID, publicationTimeFrame2.ID, gradeId)).Returns(_timeFrame2GradeAccessoriesKey);
            A.CallTo(() => keyManager.GetGradeAccessoriesKey(publication2.ID, publicationTimeFrame3.ID, gradeId)).Returns(_timeFrame3GradeAccessoriesKey);
            A.CallTo(() => keyManager.GetGradeAccessoriesKey(publication2.ID, publicationTimeFrame4.ID, gradeId)).Returns(_timeFrame4GradeAccessoriesKey);
        }

        protected override void Act()
        {
            var result = _publisher.Publish(_context).Result;
        }

        [Fact]
        public void ThenGenerationGradeAccessoriesShouldBePutForAllLanguagesAndTimeFrames()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustHaveHappened(Repeated.Exactly.Times(4));
        }

        [Fact]
        public void ThenGenerationGradeAccessoriesShouldBePutForTimeFrame1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame1GradeAccessoriesKey, _serialisedGradeAccessory1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradeAccessoriesShouldBePutForTimeFrame2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame2GradeAccessoriesKey, _serialisedGradeAccessory12))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradeAccessoriesShouldBePutForTimeFrame3()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame3GradeAccessoriesKey, _serialisedGradeAccessory34))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradeAccessoriesShouldBePutForTimeFrame4()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame4GradeAccessoriesKey, _serialisedGradeAccessory4))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
