using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
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
        private IEnumerable<Repository.Objects.Model> _models;
        private IContext _context;
        private const string Language = "language for test";
        private const string ModelName1 = "ModelForContextLanguageWithAnActiveAndADeletedPublicationForTheSameGeneration";
        private const string ModelName2 = "ModelForContextLanguageWithAActivePublicationsOfWhichTheTimeFramesDoNotSpanTheCurrentDate";
        private const string ModelName3 = "ModelForContextLanguageWithAnArchivedPublication";

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

            var languages = LanguagesBuilder.Initialize()
                // other language => we don't want to see this data
                .AddLanguage(otherLanguage)
                .AddModelToLanguage(otherLanguage, GetActiveModelForOtherLanguage())
                // language from context => we want to see the active models for this language
                .AddLanguage(Language)
                .AddModelToLanguage(Language, GetModelForContextLanguageWithAnActiveAndADeletedPublicationForTheSameGeneration())
                .AddModelToLanguage(Language, GetModelForContextLanguageWithActivePublicationsOfWhichTheTimeFramesDoNotSpanTheCurrentDate())
                .AddModelToLanguage(Language, GetModelForContextLanguageWithAnArchivedPublication())
                .Build();

            return languages;
        }

        private static Repository.Objects.Model GetActiveModelForOtherLanguage()
        {
            var generation = GenerationBuilder.Initialize().Build();

            var publication = PublicationInfoBuilder.Initialize()
                .WithGeneration(generation)
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .WithState(PublicationState.Activated)
                .Build();

            var otherLanguageModel = ModelBuilder
                .Initialize()
                .AddPublication(publication)
                .Build();

            return otherLanguageModel;
        }

        private static Repository.Objects.Model GetModelForContextLanguageWithAnActiveAndADeletedPublicationForTheSameGeneration()
        {
            var generation = GenerationBuilder.Initialize().Build();

            var activePublication = PublicationInfoBuilder.Initialize()
                .WithGeneration(generation)
                .WithDateRange(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2))
                .WithState(PublicationState.Activated)
                .Build();

            var deletedPublication = PublicationInfoBuilder.Initialize()
                .WithGeneration(generation)
                .WithDateRange(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2))
                .WithState(PublicationState.ToBeDeleted)
                .Build();

            var model = ModelBuilder.Initialize()
                .WithName(ModelName1)
                .AddPublication(activePublication)
                .AddPublication(deletedPublication)
                .Build();

            return model;
        }

        private static Repository.Objects.Model GetModelForContextLanguageWithActivePublicationsOfWhichTheTimeFramesDoNotSpanTheCurrentDate()
        {
            var generation = GenerationBuilder.Initialize().Build();

            var publicationInThePast = PublicationInfoBuilder.Initialize()
                .WithGeneration(generation)
                .WithDateRange(DateTime.Now.AddDays(-10), DateTime.Now.AddDays(-2))
                .WithState(PublicationState.Activated)
                .Build();

            var publicationInTheFuture = PublicationInfoBuilder.Initialize()
                .WithGeneration(generation)
                .WithDateRange(DateTime.Now.AddDays(3), DateTime.Now.AddDays(12))
                .WithState(PublicationState.Activated)
                .Build();

            var model = ModelBuilder.Initialize()
                .WithName(ModelName2)
                .AddPublication(publicationInThePast)
                .AddPublication(publicationInTheFuture)
                .Build();

            return model;
        }

        private static Repository.Objects.Model GetModelForContextLanguageWithAnArchivedPublication()
        {
            var generation = GenerationBuilder.Initialize().Build();

            var publication = PublicationInfoBuilder.Initialize()
                .WithGeneration(generation)
                .WithDateRange(DateTime.Now.AddDays(-10), DateTime.Now.AddDays(10))
                .WithState(PublicationState.Archived)
                .Build();

            var model = ModelBuilder.Initialize()
                .WithName(ModelName3)
                .AddPublication(publication)
                .Build();

            return model;
        }

        protected override void Act()
        {
            _models = _modelRepository.Get(_context);
        }

        [Fact]
        public void ThenItShouldOnlyReturnTheModelThatHasAnActivePublicationThatIsAvailableToday()
        {
            _models.Should().OnlyContain(m => m.Name.Equals(ModelName1)).And.HaveCount(1, "because we don't want duplicate models");
        }

        [Fact]
        public void ThenItShouldOnlyReturnTheModelsForTheContextLanguage()
        {
            _models.Should().HaveCount(3, "because only the 3 models for the context's language should appear, and not for any other languages");
        }

    }
}