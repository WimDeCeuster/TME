using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;
using TME.CarConfigurator.Repository.Objects.Packs;
using TME.CarConfigurator.Tests.Shared.TestBuilders;

namespace TME.CarConfigurator.Query.Tests.Services.GivenAPackService
{
    public class WhenGetCarPacksIsCalled : TestBase
    {
        private IPackService _packService;
        private IEnumerable<Repository.Objects.Packs.CarPack> _expectedPacks;
        private IEnumerable<Repository.Objects.Packs.CarPack> _actualPacks;
        private Context _context;

        protected override void Arrange()
        {
            const string brand = "a brand";
            const string country = "a country";

            _context = ContextBuilder.Initialize()
                .WithBrand(brand)
                .WithCountry(country)
                .Build();

            const string s3Key = "fake s3 key";
            const string serializedObject = "this object is serialized";

            _expectedPacks = new List<CarPack>
            {
                new CarPackBuilder()
                    .WithId(Guid.NewGuid())
                    .Build()
            };

            var serialiser = A.Fake<ISerialiser>();
            var service = A.Fake<IService>();
            var keyManager = A.Fake<IKeyManager>();

            A.CallTo(() => keyManager.GetCarPacksKey(A<Guid>._, A<Guid>._)).Returns(s3Key);
            A.CallTo(() => service.GetObject(brand, country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<IEnumerable<CarPack>>(serializedObject)).Returns(_expectedPacks);

            var serviceFacade = new S3ServiceFacade()
                .WithSerializer(serialiser)
                .WithService(service)
                .WithKeyManager(keyManager);

            _packService = serviceFacade.CreatePackService();
        }

        protected override void Act()
        {
            _actualPacks = _packService.GetCarPacks(Guid.NewGuid(), Guid.NewGuid(), _context);
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectPacks()
        {
            _actualPacks.Should().BeSameAs(_expectedPacks);
        }
    }
}