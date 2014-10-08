using FakeItEasy;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.QueryRepository.S3;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    internal class S3ModelRepositoryBuilder
    {
        private readonly IModelRepository _modelRepository;
        private ILanguageService _languageService = A.Fake<ILanguageService>();

        private S3ModelRepositoryBuilder()
        {
        }

        private S3ModelRepositoryBuilder(IModelRepository modelRepository)
        {
            _modelRepository = modelRepository;
        }

        public static S3ModelRepositoryBuilder InitializeFakeRepository()
        {
            var modelRepository = A.Fake<IModelRepository>();

            return new S3ModelRepositoryBuilder(modelRepository);
        }

        public static S3ModelRepositoryBuilder Initialize()
        {
            return new S3ModelRepositoryBuilder();
        }

        public S3ModelRepositoryBuilder WithLanguageService(ILanguageService languageService)
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