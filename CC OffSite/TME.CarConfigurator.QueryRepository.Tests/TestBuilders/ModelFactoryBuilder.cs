using TME.CarConfigurator.Factories;
using TME.CarConfigurator.QueryRepository.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    public class ModelFactoryBuilder
    {
        private IModelRepository _modelRepository = ModelRepositoryBuilder.InitializeFakeRepository().Build();

        public static ModelFactoryBuilder Initialize()
        {
            return new ModelFactoryBuilder();
        }

        public ModelFactoryBuilder WithModelRepository(IModelRepository modelRepository)
        {
            _modelRepository = modelRepository;

            return this;
        }

        public ModelFactory Build()
        {
            return new ModelFactory(_modelRepository);
        }
    }
}