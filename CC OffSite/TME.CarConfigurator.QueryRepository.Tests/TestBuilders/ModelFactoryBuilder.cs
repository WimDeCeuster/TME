using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Factories.Interfaces;
using TME.CarConfigurator.S3.QueryServices.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    public class ModelFactoryBuilder
    {
        private IModelService _modelService = ModelServiceBuilder.InitializeFakeService().Build();
        private IPublicationFactory _publicationFactory = PublicationFactoryBuilder.InitializeFakeFactory().Build();

        public static ModelFactoryBuilder Initialize()
        {
            return new ModelFactoryBuilder();
        }

        public ModelFactoryBuilder WithModelService(IModelService modelService)
        {
            _modelService = modelService;

            return this;
        }

        public ModelFactoryBuilder WithPublicationFactory(IPublicationFactory publicationFactory)
        {
            _publicationFactory = publicationFactory;

            return this;
        }

        public IModelFactory Build()
        {
            return new ModelFactory(_modelService, _publicationFactory);
        }
    }
}