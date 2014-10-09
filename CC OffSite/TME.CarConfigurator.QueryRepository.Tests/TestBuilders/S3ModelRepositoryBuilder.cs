using FakeItEasy;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.S3.QueryServices.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    internal class S3ModelRepositoryBuilder
    {
        private readonly IModelRepository _modelRepository;
        private IModelService _modelService = A.Fake<IModelService>();

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

        public S3ModelRepositoryBuilder WithLanguageService(IModelService modelService)
        {
            _modelService = modelService;

            return this;
        }

        public IModelRepository Build()
        {
            return _modelRepository ?? new ModelRepository(_modelService);
        }
    }
}