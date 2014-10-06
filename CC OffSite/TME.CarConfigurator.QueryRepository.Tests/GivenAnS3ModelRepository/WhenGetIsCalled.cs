using System;
using FakeItEasy;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.S3;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.CarConfigurator.QueryRepository.Tests.GivenAnS3ModelRepository
{
    public class WhenGetIsCalled : TestBase
    {
        private ModelRepository _modelRepository;
        private IModels _models;
        private IContext _context;
        private const string Language = "language for test";

        protected override void Arrange()
        {
            _context = ContextBuilder.InitializeFakeContext().WithLanguage(Language).Build();

            var languageService = ArrangeLanguageService();

            _modelRepository = new ModelRepository(languageService);
        }

        private static ILanguageService ArrangeLanguageService()
        {
            var languages = ArrangeLanguages();

            var service = A.Fake<ILanguageService>();
            A.CallTo(() => service.Get()).Returns(languages);

            return service;
        }

        private static Languages ArrangeLanguages()
        {
            const string otherLanguage = "first language";

            var generation = GenerationBuilder.Initialize().Build();
            var publication = PublicationInfoBuilder.Initialize()
                .WithGeneration(generation)
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .WithState(PublicationState.Activated)
                .Build();

            var otherLanguageModel = ModelBuilder
                .Initialize()
                .WithName("other language model")
                .AddPublication(publication)
                .Build();

            var languages = LanguagesBuilder.Initialize()
                .AddLanguage(otherLanguage)
                .AddModelToLanguage(otherLanguage, otherLanguageModel)
                .AddLanguage(Language)
                .Build();

            return languages;
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

    internal class GenerationBuilder
    {
        private readonly Generation _generation;

        private GenerationBuilder(Generation generation)
        {
            _generation = generation;
        }

        public static GenerationBuilder Initialize()
        {
            var generation = new Generation();

            return new GenerationBuilder(generation);
        }

        public Generation Build()
        {
            return _generation;
        }
    }
}