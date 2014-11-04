using System;
using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Packs;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3GradePackPublisher
{
    public class WhenPublishingGradePacks : TestBase
    {
        const String Brand = "Toyota";
        const String Country = "DE";
        const String SerialisedObject1 = "serialised gradeEquipment 1";
        const String SerialisedObject2 = "serialised gradeEquipment 2";
        const String SerialisedObject3 = "serialised gradeEquipment 3";
        const String SerialisedObject4 = "serialised gradeEquipment 4";
        const String Language1 = "lang 1";
        const String Language2 = "lang 2";
        const String TimeFrame1Grade1Key = "time frame 1 grade 1 equipments key";
        const String TimeFrame2Grade1Key = "time frame 2 grade 1 equipments key";
        const String TimeFrame2Grade2Key = "time frame 2 grade 2 equipments key";
        const String TimeFrame3Grade3Key = "time frame 3 grade 3 equipments key";
        const String TimeFrame3Grade4Key = "time frame 3 grade 4 equipments key";
        const String TimeFrame4Grade4Key = "time frame 1 grade 4 equipments key";

        IService _s3Service;
        IGradePackService _service;
        IGradePackPublisher _publisher;
        IContext _context;

        protected override void Arrange()
        {
            var gradeId1 = Guid.NewGuid();
            var gradeId2 = Guid.NewGuid();
            var gradeId3 = Guid.NewGuid();
            var gradeId4 = Guid.NewGuid();

            var pack1 = new[] { new GradePack(), new GradePack(), new GradePack() };
            var pack2 = new[] { new GradePack() };
            var pack3 = new[] { new GradePack(), new GradePack() };
            var pack4 = new[] { new GradePack() };

            var timeFrame1 = new TimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .WithGradePacks(gradeId1, pack1)
                .Build();
            var timeFrame2 = new TimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .WithGradePacks(gradeId1, pack1)
                .WithGradePacks(gradeId2, pack2)
                .Build();
            var timeFrame3 = new TimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .WithGradePacks(gradeId3, pack3)
                .WithGradePacks(gradeId4, pack4)
                .Build();
            var timeFrame4 = new TimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .WithGradePacks(gradeId4, pack4)
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
                .WithBrand(Brand)
                .WithCountry(Country)
                .WithLanguages(Language1, Language2)
                .WithPublication(Language1, publication1)
                .WithPublication(Language2, publication2)
                .WithTimeFrames(Language1, timeFrame1, timeFrame2)
                .WithTimeFrames(Language2, timeFrame3, timeFrame4)
                .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            var keyManager = A.Fake<IKeyManager>();

            _service = new GradePackService(_s3Service, serialiser, keyManager);
            _publisher = new GradePacksPublisherBuilder().WithService(_service).Build();

            A.CallTo(() => serialiser.Serialise(pack1))
                .Returns(SerialisedObject1);
            A.CallTo(() => serialiser.Serialise(pack2))
                .Returns(SerialisedObject2);
            A.CallTo(() => serialiser.Serialise(pack3))
                .Returns(SerialisedObject3);
            A.CallTo(() => serialiser.Serialise(pack4))
                .Returns(SerialisedObject4);

            A.CallTo(() => keyManager.GetGradePacksKey(publication1.ID, publicationTimeFrame1.ID, gradeId1)).Returns(TimeFrame1Grade1Key);
            A.CallTo(() => keyManager.GetGradePacksKey(publication1.ID, publicationTimeFrame2.ID, gradeId1)).Returns(TimeFrame2Grade1Key);
            A.CallTo(() => keyManager.GetGradePacksKey(publication1.ID, publicationTimeFrame2.ID, gradeId2)).Returns(TimeFrame2Grade2Key);
            A.CallTo(() => keyManager.GetGradePacksKey(publication2.ID, publicationTimeFrame3.ID, gradeId3)).Returns(TimeFrame3Grade3Key);
            A.CallTo(() => keyManager.GetGradePacksKey(publication2.ID, publicationTimeFrame3.ID, gradeId4)).Returns(TimeFrame3Grade4Key);
            A.CallTo(() => keyManager.GetGradePacksKey(publication2.ID, publicationTimeFrame4.ID, gradeId4)).Returns(TimeFrame4Grade4Key);
        }

        protected override void Act()
        {
            _publisher.PublishAsync(_context).Wait();
        }

        [Fact]
        public void ThenGenerationGradeEquipmentsShouldBePutForAllLanguagesAndTimeFrames()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustHaveHappened(Repeated.Exactly.Times(6));
        }

        [Fact]
        public void ThenGenerationGradeEquipmentsShouldBePutForTimeFrame1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(Brand, Country, TimeFrame1Grade1Key, SerialisedObject1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradeEquipmentsShouldBePutForTimeFrame2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(Brand, Country, TimeFrame2Grade1Key, SerialisedObject1))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _s3Service.PutObjectAsync(Brand, Country, TimeFrame2Grade2Key, SerialisedObject2))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradeEquipmentsShouldBePutForTimeFrame3()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(Brand, Country, TimeFrame3Grade3Key, SerialisedObject3))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _s3Service.PutObjectAsync(Brand, Country, TimeFrame3Grade4Key, SerialisedObject4))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradeEquipmentsShouldBePutForTimeFrame4()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(Brand, Country, TimeFrame4Grade4Key, SerialisedObject4))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}