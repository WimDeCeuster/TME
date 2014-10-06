using System;
using FakeItEasy;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.S3;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders.Base;
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

            var publication = PublicationInfoBuilder.Initialize().WithDateRange(DateTime.MinValue, DateTime.MaxValue).WithState(PublicationState.Activated).Build();
            var publications = PublicationsBuilder.Initialize().AddPublication(publication).Build();
            var otherLanguageModel = ModelBuilder
                .Initialize()
                .WithName("other language model")
                .WithPublications(publications)
                .Build();

            var languages = LanguagesBuilder.Initialize()
                .AddLanguage(otherLanguage)
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

    internal class PublicationInfoBuilder
    {
        private readonly Publication _publication;
        private PublicationState _publicationState;

        private PublicationInfoBuilder(Publication publication)
        {
            _publication = publication;
        }

        public static PublicationInfoBuilder Initialize()
        {
            var publication = new Publication();

            return new PublicationInfoBuilder(publication);
        }

        public PublicationInfoBuilder WithDateRange(DateTime from, DateTime to)
        {
            _publication.LineOffFrom = from;
            _publication.LineOffTo = to;

            return this;
        }

        public PublicationInfoBuilder WithState(PublicationState publicationState)
        {
            _publicationState = publicationState;

            return this;
        }

        public PublicationInfo Build()
        {
            return new PublicationInfo(_publication) {State = _publicationState};
        }
    }


    internal class ModelBuilder
    {
        private readonly Repository.Objects.Model _model;

        private ModelBuilder(Repository.Objects.Model model)
        {
            _model = model;
        }

        public static ModelBuilder Initialize()
        {
            var model = new Repository.Objects.Model();

            return new ModelBuilder(model);
        }

        public Repository.Objects.Model Build()
        {
            return _model;
        }

        public ModelBuilder WithName(string modelName)
        {
            _model.Name = modelName;

            return this;
        }
    }
}