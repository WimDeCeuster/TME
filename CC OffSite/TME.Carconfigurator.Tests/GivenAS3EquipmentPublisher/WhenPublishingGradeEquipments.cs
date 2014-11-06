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

namespace TME.Carconfigurator.Tests.GivenAS3EquipmentPublisher
{
    public class WhenPublishingGenerationGradeEquipments : TestBase
    {
        const String _brand = "Toyota";
        const String _country = "DE";
        const String _serialisedGradeEquipment = "serialised gradeEquipment";
        const String _language1 = "lang 1";
        const String _language2 = "lang 2";
        const String _timeFrame1Grade1EquipmentsKey = "time frame 1 grade 1 equipments key";
        const String _timeFrame2Grade1EquipmentsKey = "time frame 2 grade 1 equipments key";
        const String _timeFrame2Grade2EquipmentsKey = "time frame 2 grade 2 equipments key";
        const String _timeFrame3Grade3EquipmentsKey = "time frame 3 grade 3 equipments key";
        const String _timeFrame3Grade4EquipmentsKey = "time frame 3 grade 4 equipments key";
        const String _timeFrame4Grade4EquipmentsKey = "time frame 1 grade 4 equipments key";
        IService _s3Service;
        IEquipmentService _service;
        IEquipmentPublisher _publisher;
        IContext _context;

        protected override void Arrange()
        {
            var gradeId1 = Guid.NewGuid();
            var gradeId2 = Guid.NewGuid();
            var gradeId3 = Guid.NewGuid();
            var gradeId4 = Guid.NewGuid();
            
            var gradeEquipment1 = new GradeEquipmentBuilder().Build();
            var gradeEquipment2 = new GradeEquipmentBuilder().Build();
            var gradeEquipment3 = new GradeEquipmentBuilder().Build();
            var gradeEquipment4 = new GradeEquipmentBuilder().Build();

            var timeFrame1 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithGradeEquipment(gradeId1, gradeEquipment1)
                                .Build();
            var timeFrame2 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithGradeEquipment(gradeId1, gradeEquipment1)
                                .WithGradeEquipment(gradeId2, gradeEquipment2)
                                .Build();
            var timeFrame3 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithGradeEquipment(gradeId3, gradeEquipment3)
                                .WithGradeEquipment(gradeId4, gradeEquipment4)
                                .Build();
            var timeFrame4 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithGradeEquipment(gradeId4, gradeEquipment4)
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

            _service = new EquipmentService(_s3Service, serialiser, keyManager);
            _publisher = new EquipmentPublisherBuilder().WithService(_service).Build();

            A.CallTo(() => serialiser.Serialise(A<GradeEquipment>._))
                .Returns(_serialisedGradeEquipment);

            A.CallTo(() => keyManager.GetGradeEquipmentsKey(publication1.ID, publicationTimeFrame1.ID, gradeId1)).Returns(_timeFrame1Grade1EquipmentsKey);
            A.CallTo(() => keyManager.GetGradeEquipmentsKey(publication1.ID, publicationTimeFrame2.ID, gradeId1)).Returns(_timeFrame2Grade1EquipmentsKey);
            A.CallTo(() => keyManager.GetGradeEquipmentsKey(publication1.ID, publicationTimeFrame2.ID, gradeId2)).Returns(_timeFrame2Grade2EquipmentsKey);
            A.CallTo(() => keyManager.GetGradeEquipmentsKey(publication2.ID, publicationTimeFrame3.ID, gradeId3)).Returns(_timeFrame3Grade3EquipmentsKey);
            A.CallTo(() => keyManager.GetGradeEquipmentsKey(publication2.ID, publicationTimeFrame3.ID, gradeId4)).Returns(_timeFrame3Grade4EquipmentsKey);
            A.CallTo(() => keyManager.GetGradeEquipmentsKey(publication2.ID, publicationTimeFrame4.ID, gradeId4)).Returns(_timeFrame4Grade4EquipmentsKey);
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
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame1Grade1EquipmentsKey, _serialisedGradeEquipment))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradeEquipmentsShouldBePutForTimeFrame2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame2Grade1EquipmentsKey, _serialisedGradeEquipment))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame2Grade2EquipmentsKey, _serialisedGradeEquipment))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradeEquipmentsShouldBePutForTimeFrame3()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame3Grade3EquipmentsKey, _serialisedGradeEquipment))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame3Grade4EquipmentsKey, _serialisedGradeEquipment))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationGradeEquipmentsShouldBePutForTimeFrame4()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame4Grade4EquipmentsKey, _serialisedGradeEquipment))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
