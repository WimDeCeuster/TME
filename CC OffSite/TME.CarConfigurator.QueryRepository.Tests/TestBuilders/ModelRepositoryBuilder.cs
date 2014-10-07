using System;
using FakeItEasy;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.QueryRepository.S3;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    internal class ModelRepositoryBuilder
    {
        private readonly IModelRepository _modelRepository;
        private ILanguageService _languageService = A.Fake<ILanguageService>();

        private ModelRepositoryBuilder()
        {
        }

        private ModelRepositoryBuilder(IModelRepository modelRepository)
        {
            _modelRepository = modelRepository;
        }

        public static ModelRepositoryBuilder InitializeFakeRepository()
        {
            var modelRepository = A.Fake<IModelRepository>();

            return new ModelRepositoryBuilder(modelRepository);
        }

        public static ModelRepositoryBuilder Initialize()
        {
            return new ModelRepositoryBuilder();
        }

        public ModelRepositoryBuilder WithLanguageService(ILanguageService languageService)
        {
            _languageService = languageService;

            return this;
        }

        public IModelRepository Build()
        {
            return _modelRepository ?? new ModelRepository(_languageService);
        }
    }
}