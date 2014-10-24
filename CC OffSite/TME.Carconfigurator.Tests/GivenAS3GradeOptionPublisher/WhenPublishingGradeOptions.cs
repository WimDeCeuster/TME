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

namespace TME.Carconfigurator.Tests.GivenAS3GradeOptionPublisher
{
    public class WhenPublishingGenerationGradeOptions : TestBase
    {
        const String _brand = "Toyota";
        const String _country = "DE";
        const String _serialisedGradeOption1 = "serialised gradeOption 1";
        const String _serialisedGradeOption12 = "serialised gradeOption 1+2";
        const String _serialisedGradeOption34 = "serialised gradeOption 3+4";
        const String _serialisedGradeOption4 = "serialised gradeOption 4";
        const String _language1 = "lang 1";
        const String _language2 = "lang 2";
        const String _timeFrame1GradeOptionsKey = "time frame 1 gradeOptions key";
        const String _timeFrame2GradeOptionsKey = "time frame 2 gradeOptions key";
        const String _timeFrame3GradeOptionsKey = "time frame 3 gradeOptions key";
        const String _timeFrame4GradeOptionsKey = "time frame 4 gradeOptions key";
        IService _s3Service;
        IGradeOptionService _service;
        IGradeOptionPublisher _publisher;
        IContext _context;

        protected override void Arrange()
        {
            var gradeId = Guid.NewGuid();

            var gradeOption1 = new GradeOption { ID = Guid.NewGuid() };
            var gradeOption2 = new GradeOption { ID = Guid.NewGuid() };
            var gradeOption3 = new GradeOption { ID = Guid.NewGuid() };
            var gradeOption4 = new GradeOption { ID = Guid.NewGuid() };

            var timeFrame1 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithGradeOptions(gradeId, new[] { gradeOption1 })
                                .Build();
            var timeFrame2 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithGradeOptions(gradeId, new[] { gradeOption1, gradeOption2 })
                                .Build();
            var timeFrame3 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithGradeOptions(gradeId, new[] { gradeOption3, gradeOption4 })
                                .Build();
            var timeFrame4 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithGradeOptions(gradeId, new[] { gradeOption4 })
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

            _service = new GradeOptionService(_s3Service, serialiser, keyManager);
            _publisher = new GradeOptionPublisherBuilder().WithService(_service).Build();

            A.CallTo(() => serialiser.Serialise(A<IEnumerable<GradeOption>>.That.IsSameSequenceAs(new[] { gradeOption1 })))
                .Returns(_serialisedGradeOption1);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<GradeOption>>.That.IsSameSequenceAs(new[] { gradeOption1, gradeOption2 })))
                .Returns(_serialisedGradeOption12);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<GradeOption>>.That.IsSameSequenceAs(new[] { gradeOption3, gradeOption4 })))
                .Returns(_serialisedGradeOption34);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<GradeOption>>.That.IsSameSequenceAs(new[] { gradeOption4 })))
                .Returns(_serialisedGradeOption4);

            A.CallTo(() => keyManager.GetGradeOptionsKey(publication1.ID, publicationTimeFrame1.ID, gradeId)).Returns(_timeFrame1GradeOptionsKey);
            A.CallTo(() => keyManager.GetGradeOptionsKey(publication1.ID, publicationTimeFrame2.ID, gradeId)).Returns(_timeFrame2GradeOptionsKey);
            A.CallTo(() => keyManager.GetGradeOptionsKey(publication2.ID, publicationTimeFrame3.ID, gradeId)).Returns(_timeFrame3GradeOptionsKey);
            A.CallTo(() => keyManager.GetGradeOptionsKey(publication2.ID, publicationTimeFrame4.ID, gradeId)).Returns(_timeFrame4GradeOptionsKey);
        }

        protected override void Act()
        {
            var result = _publisher.Publish(_context).Result;
        }

        [Fact]
        public void ThenGenerationGradeOptionsShouldBePutForAllLanguagesAndTimeFrames()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustHaveHappened(Repeated.Exactly.Times(4));
        }

        [Fact]
        public void ThenGenerationGradeOptionsShouldBePutForTimeFrame1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame1GradeOptionsKey, _serialisedGradeOption1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradeOptionsShouldBePutForTimeFrame2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame2GradeOptionsKey, _serialisedGradeOption12))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradeOptionsShouldBePutForTimeFrame3()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame3GradeOptionsKey, _serialisedGradeOption34))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradeOptionsShouldBePutForTimeFrame4()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame4GradeOptionsKey, _serialisedGradeOption4))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
