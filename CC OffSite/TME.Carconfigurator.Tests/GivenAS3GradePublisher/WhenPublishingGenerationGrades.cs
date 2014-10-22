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

namespace TME.Carconfigurator.Tests.GivenAS3GradePublisher
{
    public class WhenPublishingGenerationGrades : TestBase
    {
        const String _brand = "Toyota";
        const String _country = "DE";
        const String _serialisedGrade1 = "serialised grade 1";
        const String _serialisedGrade12 = "serialised grade 1+2";
        const String _serialisedGrade34 = "serialised grade 3+4";
        const String _serialisedGrade4 = "serialised grade 4";
        const String _language1 = "lang 1";
        const String _language2 = "lang 2";
        const String _timeFrame1GradesKey = "time frame 1 grades key";
        const String _timeFrame2GradesKey = "time frame 2 grades key";
        const String _timeFrame3GradesKey = "time frame 3 grades key";
        const String _timeFrame4GradesKey = "time frame 4 grades key";
        IService _s3Service;
        IGradeService _service;
        IGradePublisher _publisher;
        IContext _context;

        protected override void Arrange()
        {
            var grade1 = new Grade { ID = Guid.NewGuid() };
            var grade2 = new Grade { ID = Guid.NewGuid() };
            var grade3 = new Grade { ID = Guid.NewGuid() };
            var grade4 = new Grade { ID = Guid.NewGuid() };

            var timeFrame1 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithGrades(new[] { grade1 })
                                .Build();
            var timeFrame2 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithGrades(new[] { grade1, grade2 })
                                .Build();
            var timeFrame3 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithGrades(new[] { grade3, grade4 })
                                .Build();
            var timeFrame4 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithGrades(new[] { grade4 })
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

            _service = new GradeService(_s3Service, serialiser, keyManager);
            _publisher = new GradePublisherBuilder().WithService(_service).Build();

            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Grade>>.That.IsSameSequenceAs(new[] { grade1 })))
                .Returns(_serialisedGrade1);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Grade>>.That.IsSameSequenceAs(new[] { grade1, grade2 })))
                .Returns(_serialisedGrade12);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Grade>>.That.IsSameSequenceAs(new[] { grade3, grade4 })))
                .Returns(_serialisedGrade34);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Grade>>.That.IsSameSequenceAs(new[] { grade4 })))
                .Returns(_serialisedGrade4);

            A.CallTo(() => keyManager.GetGradesKey(publication1.ID, publicationTimeFrame1.ID)).Returns(_timeFrame1GradesKey);
            A.CallTo(() => keyManager.GetGradesKey(publication1.ID, publicationTimeFrame2.ID)).Returns(_timeFrame2GradesKey);
            A.CallTo(() => keyManager.GetGradesKey(publication2.ID, publicationTimeFrame3.ID)).Returns(_timeFrame3GradesKey);
            A.CallTo(() => keyManager.GetGradesKey(publication2.ID, publicationTimeFrame4.ID)).Returns(_timeFrame4GradesKey);
        }

        protected override void Act()
        {
            var result = _publisher.PublishGenerationGrades(_context).Result;
        }

        [Fact]
        public void ThenGenerationGradesShouldBePutForAllLanguagesAndTimeFrames()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustHaveHappened(Repeated.Exactly.Times(4));
        }

        [Fact]
        public void ThenGenerationGradesShouldBePutForTimeFrame1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame1GradesKey, _serialisedGrade1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradesShouldBePutForTimeFrame2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame2GradesKey, _serialisedGrade12))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradesShouldBePutForTimeFrame3()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame3GradesKey, _serialisedGrade34))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradesShouldBePutForTimeFrame4()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame4GradesKey, _serialisedGrade4))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
