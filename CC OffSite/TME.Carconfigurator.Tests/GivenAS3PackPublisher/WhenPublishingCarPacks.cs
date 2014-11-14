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
using System.Collections.Generic;

namespace TME.Carconfigurator.Tests.GivenAS3PackPublisher
{
    public class WhenPublishingCarPacks : TestBase
    {
        const String Brand = "Toyota";
        const String Country = "DE";
        const String SerialisedObject1 = "serialised car packs 1";
        const String SerialisedObject2 = "serialised car packs 2";
        const String ObjectKey1 = "car packs key";
        const String ObjectKey2 = "car packs key";
        const String Language1 = "lang 1";
        const String Language2 = "lang 2";
        
        IService _s3Service;
        IPackService _service;
        IPackPublisher _publisher;
        IContext _context;

        protected override void Arrange()
        {
            var carId1 = Guid.NewGuid();
            var carId2 = Guid.NewGuid();

            var packs1 = new[] { new CarPack(), new CarPack(), new CarPack() };
            var packs2 = new[] { new CarPack(), new CarPack(), new CarPack() };

            var publication1 = new PublicationBuilder().WithID(Guid.NewGuid()).Build();
            var publication2 = new PublicationBuilder().WithID(Guid.NewGuid()).Build();

            _context = new ContextBuilder()
                .WithBrand(Brand)
                .WithCountry(Country)
                .WithLanguages(Language1, Language2)
                .WithPublication(Language1, publication1)
                .WithPublication(Language2, publication2)
                .WithCarPacks(Language1, carId1, packs1)
                .WithCarPacks(Language2, carId2, packs2)
                .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            var keyManager = A.Fake<IKeyManager>();

            _service = new PackService(_s3Service, serialiser, keyManager);
            _publisher = new PackPublisherBuilder().WithService(_service).Build();

            A.CallTo(() => serialiser.Serialise(A<IEnumerable<CarPack>>.That.IsSameSequenceAs(packs1)))
                .Returns(SerialisedObject1);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<CarPack>>.That.IsSameSequenceAs(packs2)))
                .Returns(SerialisedObject2);

            A.CallTo(() => keyManager.GetCarPacksKey(publication1.ID, carId1)).Returns(ObjectKey1);
            A.CallTo(() => keyManager.GetCarPacksKey(publication2.ID, carId2)).Returns(ObjectKey2);
        }

        protected override void Act()
        {
            _publisher.PublishCarPacksAsync(_context).Wait();
        }

        [Fact]
        public void ThenGenerationCarPacksShouldBePutForAllLanguages()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustHaveHappened(Repeated.Exactly.Times(2));
        }

        [Fact]
        public void ThenGenerationCarPacksShouldBePutForLanguage1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(Brand, Country, ObjectKey1, SerialisedObject1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationCarPacksShouldBePutForLanguage2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(Brand, Country, ObjectKey2, SerialisedObject2))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}