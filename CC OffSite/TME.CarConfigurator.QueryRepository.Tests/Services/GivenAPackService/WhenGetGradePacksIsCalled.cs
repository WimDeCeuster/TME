using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Packs;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.Services.GivenAPackService
{
    public class WhenGetGradePacksIsCalled : TestBase
    {
        private IPackService _packService;
        private IEnumerable<GradePack> _expectedPacks;
        private IEnumerable<GradePack> _actualPacks;
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

            _expectedPacks = new List<GradePack>
            {
                new GradePackBuilder()
                    .WithID(Guid.NewGuid())
                    .Build()
            };

            var serialiser = A.Fake<ISerialiser>();
            var service = A.Fake<IService>();
            var keyManager = A.Fake<IKeyManager>();

            A.CallTo(() => keyManager.GetGradePacksKey(A<Guid>._, A<Guid>._, A<Guid>._)).Returns(s3Key);
            A.CallTo(() => service.GetObject(brand, country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<IEnumerable<GradePack>>(serializedObject)).Returns(_expectedPacks);

            var serviceFacade = new S3ServiceFacade()
                .WithSerializer(serialiser)
                .WithService(service)
                .WithKeyManager(keyManager);

            _packService = serviceFacade.CreatePackService();
        }

        protected override void Act()
        {
            _actualPacks = _packService.GetGradePacks(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), _context);
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectPublication()
        {
            _actualPacks.Should().BeSameAs(_expectedPacks);
        }
    }
}