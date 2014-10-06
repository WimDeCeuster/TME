using FakeItEasy;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.S3;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.CarConfigurator.QueryRepository.Tests.GivenAnS3ModelRepository
{
    public class WhenGetIsCalled : TestBase
    {
        private ModelRepository _modelRepository;
        private IModels _models;
        private IContext _context;

        protected override void Arrange()
        {
            _context = ContextBuilder.FakeContext().Build();

            var languageService = ArrangeLanguageService();

            _modelRepository = new ModelRepository(languageService);
        }

        private static ILanguageService ArrangeLanguageService()
        {
            var languages = new Languages();

            var service = A.Fake<ILanguageService>();
            A.CallTo(() => service.Get()).Returns(languages);

            return service;
        }

        protected override void Act()
        {
            _models = _modelRepository.GetModels(_context);
        }

        [Fact]
        public void ThenItShouldReturnAllActiveModelsForTheCorrectLanguage()
        {
            Assert.True(false, "Test not implemented yet");
        }
    }
}