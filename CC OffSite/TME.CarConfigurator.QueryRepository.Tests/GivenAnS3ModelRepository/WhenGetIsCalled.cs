using FakeItEasy;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.CarConfigurator.QueryRepository.Tests.GivenAnS3ModelRepository
{
    public class WhenGetIsCalled : TestBase
    {
        protected override void Arrange()
        {
            var languageService = ArrangeLanguageService();
        }

        private static ILanguageService ArrangeLanguageService()
        {
            var service = A.Fake<ILanguageService>();
            var languages = new Languages();
            A.CallTo(() => service.Get()).Returns(languages);

            return service;
        }

        protected override void Act()
        {

        }

        [Fact]
        public void ThenItShouldFetchTheModelsFromTheService()
        {

            Assert.True(false, "Test not implemented yet");
        }

        [Fact]
        public void ThenItShouldReturnAllActiveModelsForTheCorrectLanguage()
        {

            Assert.True(false, "Test not implemented yet");
        }
    }
}