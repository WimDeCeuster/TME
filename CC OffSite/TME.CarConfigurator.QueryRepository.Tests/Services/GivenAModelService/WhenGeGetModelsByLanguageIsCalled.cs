using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders.RepositoryObjects;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.Services.GivenAModelService
{
    public class WhenGeGetModelsByLanguageIsCalled : TestBase
    {
        private const string Language1 = "lang 1";
        private const string Language2 = "lang 2";
        private const string Brand = "a brand";
        private const string Country = "a country";

        private IModelService _modelService;
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

            var serialiser = A.Fake<ISerialiser>();
            var keyManager = A.Fake<IKeyManager>();
            var s3Service = A.Fake<IService>();


            A.CallTo(() => keyManager.GetModelsKey()).Returns(s3Key);
            A.CallTo(() => s3Service.GetObject(Brand, Country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<Languages>(serializedObject)).Returns(_expectedLanguages);

            _modelService = ModelServiceBuilder.Initialize()
                .WithSerializer(serialiser)
                .WithService(s3Service)
                .WithKeyManager(keyManager)
                .Build();
        }

        protected override void Act()
        {
            _actualLanguages = _modelService.GetModelsByLanguage(Brand, Country);
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