using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.CarConfigurator.QueryRepository.Tests.GivenAPublicationService
{
    public class WhenGetPublicationIsCalled : TestBase
    {
        private IPublicationService _publicationService;

        protected override void Arrange()
        {
            _publicationService = PublicationServiceBuilder.Initialize().Build();
        }

        protected override void Act()
        {

        }

        [Fact]
        public void ThenItShouldReturnTheCorrectPublication()
        {
            Assert.True(false, "Test not implemented yet");
        }
    }
}