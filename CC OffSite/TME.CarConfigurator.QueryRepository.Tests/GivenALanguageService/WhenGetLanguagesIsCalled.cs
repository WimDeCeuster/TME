using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders;
using TME.CarConfigurator.Repository.Objects;
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

        private ILanguageService _languageService;
        private Languages _expectedLanguages;
        private Languages _actualLanguages;
        private IContext _context;

        protected override void Arrange()
        {
            const string brand = "a brand";
            const string country = "a country";

            _context = ContextBuilder.InitializeFakeContext()
                .WithBrand(brand)
                .WithCountry(country)
                .Build();

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
            A.CallTo(() => s3Service.GetObject(brand, country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<Languages>(serializedObject)).Returns(_expectedLanguages);

            _languageService = LanguageServiceBuilder.Initialize()
                .WithSerializer(serialiser)
                .WithService(s3Service)
                .WithKeyManager(keyManager)
                .Build();
        }

        protected override void Act()
        {
            _actualLanguages = _languageService.GetLanguages(_context);
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