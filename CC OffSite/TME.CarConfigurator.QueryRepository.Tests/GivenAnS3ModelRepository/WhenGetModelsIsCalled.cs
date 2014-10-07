using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders.RepositoryObjects;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders.S3;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.CarConfigurator.QueryRepository.Tests.GivenAnS3ModelRepository
{
    public class WhenGetModelsIsCalled : TestBase
    {
        private IModelRepository _modelRepository;
        private IEnumerable<Repository.Objects.Model> _models;
        private IContext _context;
        private const string Language = "language for test";

        protected override void Arrange()
        {
            _context = ContextBuilder.InitializeFakeContext().WithLanguage(Language).Build();

            var languageService = ArrangeLanguageService();

            _modelRepository = ModelRepositoryBuilder.Initialize().WithLanguageService(languageService).Build();
        }

        private static ILanguageService ArrangeLanguageService()
        {
            var languages = ArrangeLanguages();

            var service = A.Fake<ILanguageService>();
            A.CallTo(() => service.GetLanguages()).Returns(languages);

            return service;
        }

        private static Languages ArrangeLanguages()
        {
            const string otherLanguage = "first language";

            var languages = LanguagesBuilder.Initialize()
                // other language => we don't want to see this data
                .AddLanguage(otherLanguage)
                .AddModelToLanguage(otherLanguage, ModelBuilder.Initialize().Build())
                // language from context => we want to see the active models for this language
                .AddLanguage(Language)
                .AddModelToLanguage(Language, ModelBuilder.Initialize().Build())
                .AddModelToLanguage(Language, ModelBuilder.Initialize().Build())
                .AddModelToLanguage(Language, ModelBuilder.Initialize().Build())
                .Build();

            return languages;
        }

        protected override void Act()
        {
            _models = _modelRepository.GetModels(_context);
        }

        [Fact]
        public void ThenItShouldOnlyReturnTheModelsForTheContextLanguage()
        {
            _models.Should().HaveCount(3, "because only the 3 models for the context's language should appear, and not for any other languages");
        }

    }
}