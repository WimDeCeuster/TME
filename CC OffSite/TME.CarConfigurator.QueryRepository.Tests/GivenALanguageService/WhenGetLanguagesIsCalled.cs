using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.GetServices.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders.RepositoryObjects;
using TME.CarConfigurator.Tests.Shared.TestBuilders.S3;
using Xunit;

namespace TME.CarConfigurator.QueryRepository.Tests.GivenALanguageService
{
    public class WhenGetLanguagesIsCalled : TestBase
    {
        private const string Language1 = "lang 1";
        private const string Language2 = "lang 2";
        private const string Brand = "a brand";
        private const string Country = "a country";

        private ILanguageService _languageService;
        private Languages _expectedLanguages;
        private Languages _actualLanguages;

        protected override void Arrange()
        {

            const string s3Key = "fake s3 key";
            const string serializedObject = "this object is serialized";

            _expectedLanguages = LanguagesBuilder.Initialize()
                .AddLanguage(Language1)
                .AddLanguage(Language2)
                .Build();

            var serialiser = SerializerBuilder.InitializeFake().Build();
            var keyManager = KeyManagerBuilder.InitializeFake().Build();
            var s3Service = S3ServiceBuilder.InitializeFake().Build();


            A.CallTo(() => keyManager.GetLanguagesKey()).Returns(s3Key);
            A.CallTo(() => s3Service.GetObject(Brand, Country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<Languages>(serializedObject)).Returns(_expectedLanguages);

            _languageService = LanguageServiceBuilder.Initialize()
                .WithSerializer(serialiser)
                .WithService(s3Service)
                .WithKeyManager(keyManager)
                .Build();
        }

        protected override void Act()
        {
            _actualLanguages = _languageService.GetLanguages(Brand, Country);
        }

        [Fact]
        public void ThenItShouldGetTheLanguages()
        {
            _actualLanguages.Should().HaveCount(2);
            _actualLanguages.Should().Contain(l => l.Code == Language1);
            _actualLanguages.Should().Contain(l => l.Code == Language2);
        }
    }
}