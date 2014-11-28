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
        private const String SubmodelGradeKeyForTimeframe1Submodel1 = "SubModelGradeKeyForTimeFrame1SubModel1";
        private const String SubmodelGradeKeyForTimeframe1Submodel2 = "SubModelGradeKeyForTimeFrame1SubModel2";
        private const String SubmodelGradeKeyForTimeframe2Submodel2 = "SubModelGradeKeyForTimeFrame2SubModel2";
        private const String Serialiseddata = "SerialisedData";
        private const String Brand = "Toyota";
        private const String Country = "DE";
        private const String Language1 = "de";

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
                .WithBrand(Brand)
                .WithCountry(Country)
                .WithLanguages(Language1)
                .WithPublication(Language1,publication)
                .WithTimeFrames(Language1,timeFrame1,timeFrame2)
                .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            A.CallTo(() => serialiser.Serialise(A<List<Grade>>._)).Returns(Serialiseddata);

            var keyManager = A.Fake<IKeyManager>();
            A.CallTo(() => keyManager.GetSubModelGradesKey(publication.ID, timeFrame1.ID, subModel1.ID))
                .Returns(SubmodelGradeKeyForTimeframe1Submodel1);
            A.CallTo(() => keyManager.GetSubModelGradesKey(publication.ID, timeFrame1.ID, subModel2.ID))
                .Returns(SubmodelGradeKeyForTimeframe1Submodel2);
            A.CallTo(() => keyManager.GetSubModelGradesKey(publication.ID, timeFrame2.ID, subModel2.ID))
                .Returns(SubmodelGradeKeyForTimeframe2Submodel2);

            var gradeService = new GradeService(_s3Service,serialiser,keyManager);

            _submodelGradePublisher = new GradePublisherBuilder().WithService(gradeService).Build();
        }

        protected override void Act()
        {
            _submodelGradePublisher.PublishSubModelGradesAsync(_context).Wait();
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
                    _s3Service.PutObjectAsync(Brand, Country, SubmodelGradeKeyForTimeframe1Submodel1,
                        Serialiseddata)).MustHaveHappened(Repeated.Exactly.Once);
            
            A.CallTo(
                () =>
                    _s3Service.PutObjectAsync(Brand, Country, SubmodelGradeKeyForTimeframe1Submodel2,
                        Serialiseddata)).MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenSubmodelGradesShouldBePublishedForTimeFrame2()
        {
            A.CallTo(
                () =>
                    _s3Service.PutObjectAsync(Brand, Country, SubmodelGradeKeyForTimeframe2Submodel2,
                        Serialiseddata)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}