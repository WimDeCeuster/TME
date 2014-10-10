using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Facades;
using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator.Facades
{
    public class ModelFactoryFacade : IModelFactoryFacade
    {
        private IServiceFacade _serviceFacade;
        private IPublicationFactory _publicationFactory;

        public IModelFactoryFacade WithServiceFacade(IServiceFacade serviceFacade)
        {
            _serviceFacade = serviceFacade;

            return this;
        }

        public IModelFactoryFacade WithPublicationFactory(IPublicationFactory publicationFactory)
        {
            _publicationFactory = publicationFactory;

            return this;
        }

        public IModelFactory Create()
        {
            UseDefaultsWhenNoImplementationProvided();

            return new ModelFactory(_serviceFacade, _publicationFactory);
        }

        private void UseDefaultsWhenNoImplementationProvided()
        {
            _serviceFacade = _serviceFacade ?? new S3ServiceFacade();

            _publicationFactory = _publicationFactory ?? new PublicationFactory(_serviceFacade.CreatePublicationService());
        }
    }
}