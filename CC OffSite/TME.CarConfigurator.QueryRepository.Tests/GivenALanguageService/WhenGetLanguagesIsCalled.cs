using TME.CarConfigurator.QueryRepository.Service;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.CarConfigurator.QueryRepository.Tests.GivenALanguageService
{
    public class WhenGetLanguagesIsCalled : TestBase
    {
        protected override void Arrange()
        {
            var service = LanguageServiceBuilder.Initialize().Build();
        }

        protected override void Act()
        {
            
        }

        [Fact]
        public void ThenItShouldCreateTheCorrectKey()
        {
            Assert.True(false, "Test not implemented yet");
        }

        [Fact]
        public void ThenItShouldDeserializeTheLanguages()
        {
            Assert.True(false, "Test not implemented yet");
        }
    }

    public class LanguageServiceBuilder
    {
        public static LanguageServiceBuilder Initialize()
        {
            return new LanguageServiceBuilder();
        }

        public ILanguageService Build()
        {
            return new LanguageService();
        }
    }
}