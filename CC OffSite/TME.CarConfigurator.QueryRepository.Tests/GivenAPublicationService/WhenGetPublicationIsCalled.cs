using System;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.QueryServices.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders.RepositoryObjects;
using TME.CarConfigurator.Tests.Shared.TestBuilders.S3;
using Xunit;

namespace TME.CarConfigurator.QueryRepository.Tests.GivenAPublicationService
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

            var serialiser = SerializerBuilder.InitializeFake().Build();
            var service = S3ServiceBuilder.InitializeFake().Build();
            var keyManager = KeyManagerBuilder.InitializeFake().Build();

            A.CallTo(() => keyManager.GetPublicationKey(A<Guid>._)).Returns(s3Key);
            A.CallTo(() => service.GetObject(brand, country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<Publication>(serializedObject)).Returns(_expectedPublication);

            _publicationService = PublicationServiceBuilder.Initialize()
                .WithSerializer(serialiser)
                .WithService(service)
                .WithKeyManager(keyManager)
                .Build();
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