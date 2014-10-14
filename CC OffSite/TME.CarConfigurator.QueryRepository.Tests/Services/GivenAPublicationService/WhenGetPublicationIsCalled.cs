using System;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.Services.GivenAPublicationService
{
    public class WhenGetPublicationIsCalled : TestBase
    {
        private IPublicationService _publicationService;
        private Publication _expectedPublication;
        private Publication _actualPublication;
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

            _expectedPublication = PublicationBuilder.Initialize()
                .WithID(Guid.NewGuid())
                .Build();

            var serialiser = A.Fake<ISerialiser>();
            var service = A.Fake<IService>();
            var keyManager = A.Fake<IKeyManager>();

            A.CallTo(() => keyManager.GetPublicationKey(A<Guid>._)).Returns(s3Key);
            A.CallTo(() => service.GetObject(brand, country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<Publication>(serializedObject)).Returns(_expectedPublication);

            var serviceFacade = new S3ServiceFacade()
                .WithSerializer(serialiser)
                .WithService(service)
                .WithKeyManager(keyManager);

            _publicationService = serviceFacade.CreatePublicationService();
        }

        protected override void Act()
        {
            _actualPublication = _publicationService.GetPublication(_expectedPublication.ID, _context);
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectPublication()
        {
            _actualPublication.Should().Be(_expectedPublication);
        }
    }
}