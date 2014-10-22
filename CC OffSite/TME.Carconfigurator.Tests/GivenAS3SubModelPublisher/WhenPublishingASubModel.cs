using System;
using System.Collections.Generic;
using FakeItEasy;
using TME.CarConfigurator.CommandServices;
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

namespace TME.Carconfigurator.Tests.GivenAS3SubModelPublisher
{
    public class WhenPublishingASubModel : TestBase
    {
        private ISubModelPublisher _subModelPublisher;
        private IContext _context;
        private IService _s3Service;
        private const string TIME_FRAME1_SUB_MODEL_KEY = "Submodel Key For TimeFrame 1";
        private const string TIME_FRAME2_SUB_MODEL_KEY = "Submodel Key For TimeFrame 2";
        private const string TIME_FRAME3_SUB_MODEL_KEY = "Submodel Key For TimeFrame 3";
        private const string TIME_FRAME4_SUB_MODEL_KEY = "Submodel Key For TimeFrame 4";
        private const string TIME_FRAME1_SUB_MODEL_VALUE = "Submodel Value For TimeFrame 1";
        private const string TIME_FRAME2_SUB_MODEL_VALUE = "Submodel Value For TimeFrame 2";
        private const string TIME_FRAME3_SUB_MODEL_VALUE = "Submodel Value For TimeFrame 3";
        private const string TIME_FRAME4_SUB_MODEL_VALUE = "Submodel Value For TimeFrame 4";
        private const String BRAND = "Toyota";
        private const String COUNTRY = "DE";
        private const String LANGUAGE2 = "fr";
        private const String LANGUAGE1 = "nl";

        protected override void Arrange()
        {
            var subModel1 = new SubModelBuilder().WithID(Guid.NewGuid()).Build();
            var subModel2 = new SubModelBuilder().WithID(Guid.NewGuid()).Build();
            var subModel3 = new SubModelBuilder().WithID(Guid.NewGuid()).Build();
            var subModel4 = new SubModelBuilder().WithID(Guid.NewGuid()).Build();

            var car1 = new CarBuilder().WithId(Guid.NewGuid()).WithSubModel(subModel1).Build();
            var car2 = new CarBuilder().WithId(Guid.NewGuid()).WithSubModel(subModel2).Build();
            var car3 = new CarBuilder().WithId(Guid.NewGuid()).WithSubModel(subModel3).Build();
            var car4 = new CarBuilder().WithId(Guid.NewGuid()).WithSubModel(subModel4).Build();

            var timeFrame1 = new TimeFrame(DateTime.MinValue, DateTime.MaxValue, new[]{ car1 });
            var timeFrame2 = new TimeFrame(DateTime.MinValue, DateTime.MaxValue, new[]{ car1,car2 });
            var timeFrame3 = new TimeFrame(DateTime.MinValue, DateTime.MaxValue, new[]{ car3,car4 });
            var timeFrame4 = new TimeFrame(DateTime.MinValue, DateTime.MaxValue, new[]{ car4 });
            
            var publicationTimeFrame1 = new PublicationTimeFrameBuilder().WithID(timeFrame1.ID).WithDateRange(DateTime.MinValue,DateTime.MaxValue).Build();
            var publicationTimeFrame2 = new PublicationTimeFrameBuilder().WithID(timeFrame2.ID).WithDateRange(DateTime.MinValue,DateTime.MaxValue).Build();
            var publicationTimeFrame3 = new PublicationTimeFrameBuilder().WithID(timeFrame3.ID).WithDateRange(DateTime.MinValue,DateTime.MaxValue).Build();
            var publicationTimeFrame4 = new PublicationTimeFrameBuilder().WithID(timeFrame4.ID).WithDateRange(DateTime.MinValue,DateTime.MaxValue).Build();

            var publication1 = new PublicationBuilder().WithID(Guid.NewGuid()).WithTimeFrames(publicationTimeFrame1,publicationTimeFrame2).Build();
            var publication2 = new PublicationBuilder().WithID(Guid.NewGuid()).WithTimeFrames(publicationTimeFrame3,publicationTimeFrame4).Build();

            var generationSubModel1 = new SubModelBuilder().WithID(subModel1.ID).Build();
            var generationSubModel2 = new SubModelBuilder().WithID(subModel2.ID).Build();
            var generationSubModel3 = new SubModelBuilder().WithID(subModel3.ID).Build();
            var generationSubModel4 = new SubModelBuilder().WithID(subModel4.ID).Build();

            _context = new ContextBuilder()
                .WithBrand(BRAND)
                .WithCountry(COUNTRY)
                .WithLanguages(LANGUAGE1,LANGUAGE2)
                .WithPublication(LANGUAGE1,publication1)
                .WithPublication(LANGUAGE2,publication2)
                .WithCars(LANGUAGE1,car1,car2)
                .WithCars(LANGUAGE2,car3,car4)
                .WithTimeFrames(LANGUAGE1,timeFrame1,timeFrame2)
                .WithTimeFrames(LANGUAGE2,timeFrame3,timeFrame4)
                .WithSubModels(LANGUAGE1,generationSubModel1,generationSubModel2)
                .WithSubModels(LANGUAGE2,generationSubModel3,generationSubModel4)
                .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();

            A.CallTo(() => serialiser.Serialise(A<List<SubModel>>.That.IsSameSequenceAs(new List<SubModel> {generationSubModel1})))
                .Returns(TIME_FRAME1_SUB_MODEL_VALUE);
            A.CallTo(() => serialiser.Serialise(A<List<SubModel>>.That.IsSameSequenceAs(new List<SubModel> {generationSubModel1,generationSubModel2})))
                .Returns(TIME_FRAME2_SUB_MODEL_VALUE);
            A.CallTo(() => serialiser.Serialise(A<List<SubModel>>.That.IsSameSequenceAs(new List<SubModel> {generationSubModel3,generationSubModel4})))
                .Returns(TIME_FRAME3_SUB_MODEL_VALUE);
            A.CallTo(() => serialiser.Serialise(A<List<SubModel>>.That.IsSameSequenceAs(new List<SubModel> {generationSubModel4})))
                .Returns(TIME_FRAME4_SUB_MODEL_VALUE);

            var keyManager = A.Fake<IKeyManager>();
            A.CallTo(() => keyManager.GetSubModelKey(publication1.ID, publicationTimeFrame1.ID))
                .Returns(TIME_FRAME1_SUB_MODEL_KEY); 
            A.CallTo(() => keyManager.GetSubModelKey(publication1.ID, publicationTimeFrame2.ID))
                .Returns(TIME_FRAME2_SUB_MODEL_KEY);
            A.CallTo(() => keyManager.GetSubModelKey(publication2.ID, publicationTimeFrame3.ID))
                .Returns(TIME_FRAME3_SUB_MODEL_KEY);
            A.CallTo(() => keyManager.GetSubModelKey(publication2.ID, publicationTimeFrame4.ID))
                .Returns(TIME_FRAME4_SUB_MODEL_KEY);

            ISubModelService subModelService = new SubModelService(_s3Service,serialiser,keyManager);

            _subModelPublisher = new SubModelPublisherBuilder().WithService(subModelService).Build();
        }

        protected override void Act()
        {
            var result = _subModelPublisher.PublishGenerationSubModelsAsync(_context).Result;
        }

        [Fact]
        public void ThenGenerationSubModelsShouldBePutForEveryLanguageAndTimeFrames()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<string>._, A<string>._, A<string>._, A<string>._))
                .WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Times(4));
        }

        [Fact]
        public void ThenGenerationSubModelsShouldBePutForTimeFrame1()
        {
            A.CallTo(
                () => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME1_SUB_MODEL_KEY, TIME_FRAME1_SUB_MODEL_VALUE))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenGenerationSubModelsShouldBePutForTimeFrame2()
        {
            A.CallTo(
                () => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME2_SUB_MODEL_KEY, TIME_FRAME2_SUB_MODEL_VALUE))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenGenerationSubModelsShouldBePutForTimeFrame3()
        {
            A.CallTo(
                () => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME3_SUB_MODEL_KEY, TIME_FRAME3_SUB_MODEL_VALUE))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenGenerationSubModelsShouldBePutForTimeFrame4()
        {
            A.CallTo(
                () => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME4_SUB_MODEL_KEY, TIME_FRAME4_SUB_MODEL_VALUE))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}