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
        const String _language1EquipmentsKey = "language 1 equipment categories key";
        const String _language2EquipmentsKey = "language 2 equipment categories key";
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
            
            var publication1 = new PublicationBuilder()
                                .WithID(Guid.NewGuid())
                                .Build();

            var publication2 = new PublicationBuilder()
                                .WithID(Guid.NewGuid())
                                .Build();

            _context = new ContextBuilder()
                        .WithBrand(_brand)
                        .WithCountry(_country)
                        .WithLanguages(_language1, _language2)
                        .WithPublication(_language1, publication1)
                        .WithPublication(_language2, publication2)
                        .WithEquipmentCategories(_language1, equipmentCategory1, equipmentCategory2)
                        .WithEquipmentCategories(_language2, equipmentCategory3, equipmentCategory4)
                        .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            var keyManager = A.Fake<IKeyManager>();

            _service = new EquipmentService(_s3Service, serialiser, keyManager);
            _publisher = new EquipmentPublisherBuilder().WithService(_service).Build();

            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Category>>._))
                .Returns(_serialisedEquipmentCategories);

            A.CallTo(() => keyManager.GetEquipmentCategoriesKey(publication1.ID)).Returns(_language1EquipmentsKey);
            A.CallTo(() => keyManager.GetEquipmentCategoriesKey(publication2.ID)).Returns(_language2EquipmentsKey);
        }

        protected override void Act()
        {
             _publisher.PublishCategoriesAsync(_context).Wait();
        }

        [Fact]
        public void ThenGenerationEquipmentCategoriesShouldBePutForAllLanguages()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustHaveHappened(Repeated.Exactly.Times(2));
        }

        [Fact]
        public void ThenGenerationEquipmentCategoriesShouldBePutForLanguage1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _language1EquipmentsKey, _serialisedEquipmentCategories))
                .MustHaveHappened(Repeated.Exactly.Once);                                                   
        }                                                                                                   
                                                                                                            
        [Fact]                                                                                              
        public void ThenGenerationGradeEquipmentsShouldBePutForLanguage2()                                 
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _language2EquipmentsKey, _serialisedEquipmentCategories))
                .MustHaveHappened(Repeated.Exactly.Once);                                                   
        }
    }
}
