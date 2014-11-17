using FakeItEasy;
using System;
using System.Collections.Generic;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.CommandServices;

namespace TME.Carconfigurator.Tests.GivenAS3GradePublisher
{
    public class WhenPublishingGenerationGrades : TestBase
    {
        const String BRAND = "Toyota";
        const String COUNTRY = "DE";
        const String SERIALISED_GRADE1 = "serialised grade 1";
        const String SERIALISED_GRADE12 = "serialised grade 1+2";
        const String SERIALISED_GRADE34 = "serialised grade 3+4";
        const String SERIALISED_GRADE4 = "serialised grade 4";
        const String LANGUAGE1 = "lang 1";
        const String LANGUAGE2 = "lang 2";
        const String TIME_FRAME1_GRADES_KEY = "time frame 1 grades key";
        const String TIME_FRAME2_GRADES_KEY = "time frame 2 grades key";
        const String TIME_FRAME3_GRADES_KEY = "time frame 3 grades key";
        const String TIME_FRAME4_GRADES_KEY = "time frame 4 grades key";
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

            var publication1 = new PublicationBuilder()
                                .WithTimeFrames(publicationTimeFrame1,
                                                publicationTimeFrame2)
                                .Build();

            var publication2 = new PublicationBuilder()
                                .WithTimeFrames(publicationTimeFrame3,
                                                publicationTimeFrame4)
                                .Build();

            _context = new ContextBuilder()
                        .WithBrand(BRAND)
                        .WithCountry(COUNTRY)
                        .WithLanguages(LANGUAGE1, LANGUAGE2)
                        .WithPublication(LANGUAGE1, publication1)
                        .WithPublication(LANGUAGE2, publication2)
                        .WithTimeFrames(LANGUAGE1, timeFrame1, timeFrame2)
                        .WithTimeFrames(LANGUAGE2, timeFrame3, timeFrame4)
                        .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            var keyManager = A.Fake<IKeyManager>();

            _service = new GradeService(_s3Service, serialiser, keyManager);
            _publisher = new GradePublisherBuilder().WithService(_service).Build();

            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Grade>>.That.IsSameSequenceAs(new[] { grade1 })))
                .Returns(SERIALISED_GRADE1);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Grade>>.That.IsSameSequenceAs(new[] { grade1, grade2 })))
                .Returns(SERIALISED_GRADE12);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Grade>>.That.IsSameSequenceAs(new[] { grade3, grade4 })))
                .Returns(SERIALISED_GRADE34);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Grade>>.That.IsSameSequenceAs(new[] { grade4 })))
                .Returns(SERIALISED_GRADE4);

            A.CallTo(() => keyManager.GetGradesKey(publication1.ID, publicationTimeFrame1.ID)).Returns(TIME_FRAME1_GRADES_KEY);
            A.CallTo(() => keyManager.GetGradesKey(publication1.ID, publicationTimeFrame2.ID)).Returns(TIME_FRAME2_GRADES_KEY);
            A.CallTo(() => keyManager.GetGradesKey(publication2.ID, publicationTimeFrame3.ID)).Returns(TIME_FRAME3_GRADES_KEY);
            A.CallTo(() => keyManager.GetGradesKey(publication2.ID, publicationTimeFrame4.ID)).Returns(TIME_FRAME4_GRADES_KEY);
        }

        protected override void Act()
        {
            _publisher.PublishGenerationGradesAsync(_context).Wait();
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
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME1_GRADES_KEY, SERIALISED_GRADE1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradesShouldBePutForTimeFrame2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME2_GRADES_KEY, SERIALISED_GRADE12))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradesShouldBePutForTimeFrame3()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME3_GRADES_KEY, SERIALISED_GRADE34))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradesShouldBePutForTimeFrame4()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME4_GRADES_KEY, SERIALISED_GRADE4))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
