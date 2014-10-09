using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.GetServices.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders.RepositoryObjects;
using Xunit;

namespace TME.CarConfigurator.QueryRepository.Tests.GivenAnS3ModelRepository
{
    public class WhenGetModelsIsCalled : TestBase
    {
        private IModelRepository _modelRepository;
        private IEnumerable<Repository.Objects.Model> _models;
        private Context _context;
        private const string Language = "language for test";

        protected override void Arrange()
        {
            _context = ContextBuilder.Initialize().WithLanguage(Language).Build();

            var languageService = ArrangeLanguageService();

            _modelRepository = S3ModelRepositoryBuilder.Initialize().WithLanguageService(languageService).Build();
        }

        private ILanguageService ArrangeLanguageService()
        {
            var languages = ArrangeLanguages();

            var service = A.Fake<ILanguageService>();
            A.CallTo(() => service.GetLanguages(_context.Brand, _context.Country)).Returns(languages);

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