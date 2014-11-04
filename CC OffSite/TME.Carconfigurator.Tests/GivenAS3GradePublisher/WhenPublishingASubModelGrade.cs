using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3GradePublisher
{
    public class WhenPublishingASubModelGrade : TestBase
    {
        private IGradePublisher _submodelGradePublisher;
        private IService _s3Service;
        private IContext _context;
        private const String SUBMODEL_GRADE_KEY_FOR_TIMEFRAME1_SUBMODEL1 = "SubModelGradeKeyForTimeFrame1SubModel1";
        private const String SUBMODEL_GRADE_KEY_FOR_TIMEFRAME1_SUBMODEL2 = "SubModelGradeKeyForTimeFrame1SubModel2";
        private const String SUBMODEL_GRADE_KEY_FOR_TIMEFRAME2_SUBMODEL2 = "SubModelGradeKeyForTimeFrame2SubModel2";
        private const String SERIALISEDDATA = "SerialisedData";
        private const String BRAND = "Toyota";
        private const String COUNTRY = "DE";
        private const String LANGUAGE1 = "de";

        protected override void Arrange()
        {
            var grade1 = new GradeBuilder().WithId(Guid.NewGuid()).Build();
            var grade2 = new GradeBuilder().WithId(Guid.NewGuid()).Build();
            var grade3 = new GradeBuilder().WithId(Guid.NewGuid()).Build();
            var grade4 = new GradeBuilder().WithId(Guid.NewGuid()).Build();

            var subModel1 = new SubModelBuilder().WithID(Guid.NewGuid()).Build();
            var subModel2 = new SubModelBuilder().WithID(Guid.NewGuid()).Build();

            var timeFrame1 = new TimeFrameBuilder()
                .WithSubModels(new [] {subModel1,subModel2})
                .WithDateRange(DateTime.MinValue,DateTime.MaxValue)
                .WithSubModelGrades(subModel1,new [] {grade1,grade2})
                .WithSubModelGrades(subModel2,new[]{grade3,grade4})
                .Build();

            var timeFrame2 = new TimeFrameBuilder()
                .WithSubModels(new[] { subModel2 })
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .WithSubModelGrades(subModel2, new[] { grade3, grade4 })
                .Build();

            var publicationTimeFrame1 = new PublicationTimeFrameBuilder().WithID(timeFrame1.ID).Build();
            var publicationTimeFrame2 = new PublicationTimeFrameBuilder().WithID(timeFrame2.ID).Build();

            var publication = new PublicationBuilder()
                .WithTimeFrames(publicationTimeFrame1, publicationTimeFrame2)
                .Build();

            _context = new ContextBuilder()
                .WithBrand(BRAND)
                .WithCountry(COUNTRY)
                .WithLanguages(LANGUAGE1)
                .WithPublication(LANGUAGE1,publication)
                .WithTimeFrames(LANGUAGE1,timeFrame1,timeFrame2)
                .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            A.CallTo(() => serialiser.Serialise(A<List<Grade>>._)).Returns(SERIALISEDDATA);

            var keyManager = A.Fake<IKeyManager>();
            A.CallTo(() => keyManager.GetSubModelGradesKey(publication.ID, timeFrame1.ID, subModel1.ID))
                .Returns(SUBMODEL_GRADE_KEY_FOR_TIMEFRAME1_SUBMODEL1);
            A.CallTo(() => keyManager.GetSubModelGradesKey(publication.ID, timeFrame1.ID, subModel2.ID))
                .Returns(SUBMODEL_GRADE_KEY_FOR_TIMEFRAME1_SUBMODEL2);
            A.CallTo(() => keyManager.GetSubModelGradesKey(publication.ID, timeFrame2.ID, subModel2.ID))
                .Returns(SUBMODEL_GRADE_KEY_FOR_TIMEFRAME2_SUBMODEL2);

            var gradeService = new GradeService(_s3Service,serialiser,keyManager);

            _submodelGradePublisher = new GradePublisherBuilder().WithService(gradeService).Build();
        }

        protected override void Act()
        {
            var result = _submodelGradePublisher.PublishSubModelGradesAsync(_context).Result;
        }

        [Fact]
        public void ThenSubModelGradesShouldBePublishedForEveryTimeFrameAndSubModel()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<String>._,A<String>._,A<String>._,A<String>._))
                .MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public void ThenSubmodelGradesShouldBePublishedForTimeFrame1()
        {
            A.CallTo(
                () =>
                    _s3Service.PutObjectAsync(BRAND, COUNTRY, SUBMODEL_GRADE_KEY_FOR_TIMEFRAME1_SUBMODEL1,
                        SERIALISEDDATA)).MustHaveHappened(Repeated.Exactly.Once);
            
            A.CallTo(
                () =>
                    _s3Service.PutObjectAsync(BRAND, COUNTRY, SUBMODEL_GRADE_KEY_FOR_TIMEFRAME1_SUBMODEL2,
                        SERIALISEDDATA)).MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenSubmodelGradesShouldBePublishedForTimeFrame2()
        {
            A.CallTo(
                () =>
                    _s3Service.PutObjectAsync(BRAND, COUNTRY, SUBMODEL_GRADE_KEY_FOR_TIMEFRAME2_SUBMODEL2,
                        SERIALISEDDATA)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}