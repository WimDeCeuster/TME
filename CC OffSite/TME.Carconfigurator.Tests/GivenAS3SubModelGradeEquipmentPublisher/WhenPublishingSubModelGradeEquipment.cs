using System;
using System.Threading.Tasks;
using FakeItEasy;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Publisher.Interfaces;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3SubModelGradeEquipmentPublisher
{
    public class WhenPublishingSubModelGradeEquipment : TestBase
    {
        private IContext _context;
        private IEquipmentPublisher _subModelGradeEquipmentPublisher;
        private IService _s3Service;
        private const string TIME_FRAME_1_GRADE_1_SUBMODEL1_EQUIPMENTITEM_KEY = "Timeframe 1, Grade 1, Submodel 1, EquipmentKey";
        private const string TIME_FRAME_2_GRADE_1_SUBMODEL1_EQUIPMENTITEM_KEY = "Timeframe 2, Grade 1, Submodel 1, EquipmentKey";
        private const string TIME_FRAME_2_GRADE_1_SUBMODEL2_EQUIPMENTITEM_KEY = "Timeframe 2, Grade 1, Submodel 2, EquipmentKey";
        private const string TIME_FRAME_2_GRADE_2_SUBMODEL1_EQUIPMENTITEM_KEY = "Timeframe 2, Grade 2, Submodel 1, EquipmentKey";
        private const string TIME_FRAME_2_GRADE_2_SUBMODEL2_EQUIPMENTITEM_KEY = "Timeframe 2, Grade 2, Submodel 2, EquipmentKey";
        private const string TIME_FRAME_3_GRADE_3_SUBMODEL3_EQUIPMENTITEM_KEY = "Timeframe 3, Grade 3, Submodel 3, EquipmentKey";
        private const string TIME_FRAME_3_GRADE_3_SUBMODEL4_EQUIPMENTITEM_KEY = "Timeframe 3, Grade 3, Submodel 4, EquipmentKey";
        private const string TIME_FRAME_3_GRADE_4_SUBMODEL3_EQUIPMENTITEM_KEY = "Timeframe 3, Grade 4, Submodel 3, EquipmentKey";
        private const string TIME_FRAME_3_GRADE_4_SUBMODEL4_EQUIPMENTITEM_KEY = "Timeframe 3, Grade 4, Submodel 4, EquipmentKey";
        private const string TIME_FRAME_4_GRADE_4_SUBMODEL4_EQUIPMENTITEM_KEY = "Timeframe 4, Grade 4, Submodel 4, EquipmentKey";
        const string BRAND = "Toyota";
        const string COUNTRY = "DE";
        const string LANGUAGE1 = "de";
        const string LANGUAGE2 = "en";
        const string SERIALISED_EQUIPMENTITEM = "Serialised EquipmentItem";

        protected override void Arrange()
        {
            var grade1 = new GradeBuilder().WithId(Guid.NewGuid()).Build();
            var grade2 = new GradeBuilder().WithId(Guid.NewGuid()).Build();
            var grade3 = new GradeBuilder().WithId(Guid.NewGuid()).Build();
            var grade4 = new GradeBuilder().WithId(Guid.NewGuid()).Build();

            var gradeEquipment1 = new GradeEquipmentBuilder().Build();
            var gradeEquipment2 = new GradeEquipmentBuilder().Build();
            var gradeEquipment3 = new GradeEquipmentBuilder().Build();
            var gradeEquipment4 = new GradeEquipmentBuilder().Build();

            var subModel1 = new SubModelBuilder().WithID(Guid.NewGuid()).WithGrades(new[] {grade1}).Build();
            var subModel2 = new SubModelBuilder().WithID(Guid.NewGuid()).WithGrades(new[] { grade1, grade2 }).Build();
            var subModel3 = new SubModelBuilder().WithID(Guid.NewGuid()).WithGrades(new[] { grade3, grade4 }).Build();
            var subModel4 = new SubModelBuilder().WithID(Guid.NewGuid()).WithGrades(new[] {grade4}).Build();

            var timeFrame1 = new TimeFrameBuilder()
                .WithGradeEquipment(grade1.ID,gradeEquipment1)
                .WithDateRange(DateTime.MinValue,DateTime.MaxValue)
                .WithSubModels(new [] { subModel1 })
                .Build();

            var timeFrame2 = new TimeFrameBuilder()
                .WithGradeEquipment(grade1.ID, gradeEquipment1)
                .WithGradeEquipment(grade2.ID, gradeEquipment2)
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .WithSubModels(new[] { subModel1, subModel2 })
                .Build();
            
            var timeFrame3 = new TimeFrameBuilder()
                .WithGradeEquipment(grade3.ID, gradeEquipment3)
                .WithGradeEquipment(grade4.ID, gradeEquipment4)
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .WithSubModels(new[] { subModel3, subModel4 })
                .Build();
            
            var timeFrame4 = new TimeFrameBuilder()
                .WithGradeEquipment(grade4.ID, gradeEquipment4)
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .WithSubModels(new[] { subModel4 })
                .Build();

            var publicationTimeFrame1 = new PublicationTimeFrameBuilder().WithID(timeFrame1.ID).Build();
            var publicationTimeFrame2 = new PublicationTimeFrameBuilder().WithID(timeFrame2.ID).Build();
            var publicationTimeFrame3 = new PublicationTimeFrameBuilder().WithID(timeFrame3.ID).Build();
            var publicationTimeFrame4 = new PublicationTimeFrameBuilder().WithID(timeFrame4.ID).Build();

            var publication1 = new PublicationBuilder().WithID(Guid.NewGuid()).WithTimeFrames(publicationTimeFrame1, publicationTimeFrame2).Build();
            var publication2 = new PublicationBuilder().WithID(Guid.NewGuid()).WithTimeFrames(publicationTimeFrame3, publicationTimeFrame4).Build();

            _context = new ContextBuilder()
                .WithBrand(BRAND)
                .WithCountry(COUNTRY)
                .WithLanguages(LANGUAGE1,LANGUAGE2)
                .WithPublication(LANGUAGE1, publication1)
                .WithPublication(LANGUAGE2, publication2)
                .WithTimeFrames(LANGUAGE1, timeFrame1, timeFrame2)
                .WithTimeFrames(LANGUAGE2, timeFrame3, timeFrame4)
                .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            A.CallTo(() => serialiser.Serialise(A<GradeEquipment>._)).Returns(SERIALISED_EQUIPMENTITEM);

            var keymanager = A.Fake<IKeyManager>();
            A.CallTo(() => keymanager.GetSubModelGradeEquipmentsKey(publication1.ID, timeFrame1.ID, grade1.ID, subModel1.ID)).Returns(TIME_FRAME_1_GRADE_1_SUBMODEL1_EQUIPMENTITEM_KEY);

            A.CallTo(() => keymanager.GetSubModelGradeEquipmentsKey(publication1.ID, timeFrame2.ID, grade1.ID, subModel1.ID)).Returns(TIME_FRAME_2_GRADE_1_SUBMODEL1_EQUIPMENTITEM_KEY);
            A.CallTo(() => keymanager.GetSubModelGradeEquipmentsKey(publication1.ID, timeFrame2.ID, grade1.ID, subModel2.ID)).Returns(TIME_FRAME_2_GRADE_1_SUBMODEL2_EQUIPMENTITEM_KEY);
            A.CallTo(() => keymanager.GetSubModelGradeEquipmentsKey(publication1.ID, timeFrame2.ID, grade2.ID, subModel2.ID)).Returns(TIME_FRAME_2_GRADE_2_SUBMODEL2_EQUIPMENTITEM_KEY);

            A.CallTo(() => keymanager.GetSubModelGradeEquipmentsKey(publication2.ID, timeFrame3.ID, grade3.ID, subModel3.ID)).Returns(TIME_FRAME_3_GRADE_3_SUBMODEL3_EQUIPMENTITEM_KEY);
            A.CallTo(() => keymanager.GetSubModelGradeEquipmentsKey(publication2.ID, timeFrame3.ID, grade4.ID, subModel3.ID)).Returns(TIME_FRAME_3_GRADE_4_SUBMODEL3_EQUIPMENTITEM_KEY);
            A.CallTo(() => keymanager.GetSubModelGradeEquipmentsKey(publication2.ID, timeFrame3.ID, grade4.ID, subModel4.ID)).Returns(TIME_FRAME_3_GRADE_4_SUBMODEL4_EQUIPMENTITEM_KEY);

            A.CallTo(() => keymanager.GetSubModelGradeEquipmentsKey(publication2.ID, timeFrame4.ID, grade4.ID, subModel4.ID)).Returns(TIME_FRAME_4_GRADE_4_SUBMODEL4_EQUIPMENTITEM_KEY);

            var gradeEquipmentService = new EquipmentService(_s3Service,serialiser,keymanager);
            _subModelGradeEquipmentPublisher =
                new EquipmentPublisherBuilder().WithService(gradeEquipmentService).Build();
        }

        protected override void Act()
        {
            var result = _subModelGradeEquipmentPublisher.PublishSubModelGradeEquipmentAsync(_context).Result;
        }

        [Fact]
        public void ThenSubModelGradeEquipmentShouldBePutForAllLanguagesAndTimeFramesAndSubModels()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<String>._,A<String>._,A<String>._,A<String>._))
                .MustHaveHappened(Repeated.Exactly.Times(8));
        }

        [Fact]
        public void ThenSubModelGradeEquipmentShouldBePutForTimeFrame1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME_1_GRADE_1_SUBMODEL1_EQUIPMENTITEM_KEY,SERIALISED_EQUIPMENTITEM))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenSubModelGradeEquipmentShouldBePutForTimeFrame2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME_2_GRADE_1_SUBMODEL1_EQUIPMENTITEM_KEY,SERIALISED_EQUIPMENTITEM))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME_2_GRADE_1_SUBMODEL2_EQUIPMENTITEM_KEY,SERIALISED_EQUIPMENTITEM))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME_2_GRADE_2_SUBMODEL2_EQUIPMENTITEM_KEY, SERIALISED_EQUIPMENTITEM))
                .MustHaveHappened(Repeated.Exactly.Once);
        }   
        
        [Fact]
        public void ThenSubModelGradeEquipmentShouldBePutForTimeFrame3()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME_3_GRADE_3_SUBMODEL3_EQUIPMENTITEM_KEY,SERIALISED_EQUIPMENTITEM))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME_3_GRADE_4_SUBMODEL3_EQUIPMENTITEM_KEY, SERIALISED_EQUIPMENTITEM))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME_3_GRADE_4_SUBMODEL4_EQUIPMENTITEM_KEY, SERIALISED_EQUIPMENTITEM))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenSubModelGradeEquipmentShouldBePutForTimeFrame4()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME_4_GRADE_4_SUBMODEL4_EQUIPMENTITEM_KEY, SERIALISED_EQUIPMENTITEM))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}