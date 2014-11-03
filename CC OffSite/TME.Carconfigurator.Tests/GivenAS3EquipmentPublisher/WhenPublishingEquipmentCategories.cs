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
    public class WhenPublishingEquipmentCategories : TestBase
    {
        const String _brand = "Toyota";
        const String _country = "DE";
        const String _serialisedEquipmentCategories = "serialised equipment categories";
        const String _language1 = "lang 1";
        const String _language2 = "lang 2";
        const String _timeFrame1EquipmentsKey = "time frame 1 equipment categories key";
        const String _timeFrame2EquipmentsKey = "time frame 2 equipment categories key";
        const String _timeFrame3EquipmentsKey = "time frame 3 equipment categories key";
        const String _timeFrame4EquipmentsKey = "time frame 4 equipment categories key";
        IService _s3Service;
        IEquipmentService _service;
        IEquipmentPublisher _publisher;
        IContext _context;

        protected override void Arrange()
        {
            var equipmentCategory1 = new EquipmentCategoryBuilder().Build();
            var equipmentCategory2 = new EquipmentCategoryBuilder().Build();
            var equipmentCategory3 = new EquipmentCategoryBuilder().Build();
            var equipmentCategory4 = new EquipmentCategoryBuilder().Build();

            var timeFrame1 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithEquipmentCategories(equipmentCategory1)
                                .Build();
            var timeFrame2 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithEquipmentCategories(equipmentCategory1, equipmentCategory2)
                                .Build();
            var timeFrame3 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithEquipmentCategories(equipmentCategory3, equipmentCategory4)
                                .Build();
            var timeFrame4 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithEquipmentCategories(equipmentCategory4)
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

            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Category>>._))
                .Returns(_serialisedEquipmentCategories);

            A.CallTo(() => keyManager.GetEquipmentCategoriesKey(publication1.ID, publicationTimeFrame1.ID)).Returns(_timeFrame1EquipmentsKey);
            A.CallTo(() => keyManager.GetEquipmentCategoriesKey(publication1.ID, publicationTimeFrame2.ID)).Returns(_timeFrame2EquipmentsKey);
            A.CallTo(() => keyManager.GetEquipmentCategoriesKey(publication2.ID, publicationTimeFrame3.ID)).Returns(_timeFrame3EquipmentsKey);
            A.CallTo(() => keyManager.GetEquipmentCategoriesKey(publication2.ID, publicationTimeFrame4.ID)).Returns(_timeFrame4EquipmentsKey);
        }

        protected override void Act()
        {
            var result = _publisher.PublishCategoriesAsync(_context).Result;
        }

        [Fact]
        public void ThenGenerationEquipmentCategoriesShouldBePutForAllLanguagesAndTimeFrames()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustHaveHappened(Repeated.Exactly.Times(4));
        }

        [Fact]
        public void ThenGenerationEquipmentCategoriesShouldBePutForTimeFrame1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame1EquipmentsKey, _serialisedEquipmentCategories))
                .MustHaveHappened(Repeated.Exactly.Once);                                                   
        }                                                                                                   
                                                                                                            
        [Fact]                                                                                              
        public void ThenGenerationGradeEquipmentsShouldBePutForTimeFrame2()                                 
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame2EquipmentsKey, _serialisedEquipmentCategories))
                .MustHaveHappened(Repeated.Exactly.Once);                                                   
        }                                                                                                   
                                                                                                            
        [Fact]                                                                                              
        public void ThenGenerationGradeEquipmentsShouldBePutForTimeFrame3()                                 
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame3EquipmentsKey, _serialisedEquipmentCategories))
                .MustHaveHappened(Repeated.Exactly.Once);                                                   
        }                                                                                                   
                                                                                                            
        [Fact]                                                                                              
        public void ThenGenerationGradeEquipmentsShouldBePutForTimeFrame4()                                 
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame4EquipmentsKey, _serialisedEquipmentCategories))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
