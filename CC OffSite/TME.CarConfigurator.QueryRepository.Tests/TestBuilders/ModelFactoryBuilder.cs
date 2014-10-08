using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Factories.Interfaces;
using TME.CarConfigurator.QueryRepository.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    public class ModelFactoryBuilder
    {
        private IModelRepository _modelRepository = S3ModelRepositoryBuilder.InitializeFakeRepository().Build();
        private IPublicationFactory _publicationFactory = PublicationFactoryBuilder.InitializeFakeFactory().Build();

        public static ModelFactoryBuilder Initialize()
        {
            return new ModelFactoryBuilder();
        }

        public ModelFactoryBuilder WithModelRepository(IModelRepository modelRepository)
        {
            _modelRepository = modelRepository;

            return this;
        }

        public ModelFactoryBuilder WithPublicationFactory(IPublicationFactory publicationFactory)
        {
            _publicationFactory = publicationFactory;

            return this;
        }

        public IModelFactory Build()
        {
            return new ModelFactory(_modelRepository, _publicationFactory);
        }
    }
}