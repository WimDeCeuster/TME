using TME.CarConfigurator.DI.Interfaces;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.DI
{
    public class ModelFactoryFacade : IModelFactoryFacade
    {
        private IServiceFacade _serviceFacade;
        private IModelService _modelService;
        private IPublicationFactory _publicationFactory;
        private IAssetFactory _assetFactory;
        private ILinkFactory _linkFactory;

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

        public IModelFactoryFacade WithAssetFactory(IAssetFactory assetFactory)
        {
            _assetFactory = assetFactory;

            return this;
        }

        public IModelFactoryFacade WithLinkFactory(ILinkFactory linkFactory)
        {
            _linkFactory = linkFactory;

            return this;
        }

        public IModelFactory Create()
        {
            UseDefaultsWhenNoImplementationProvided();

            return new ModelFactory(_modelService, _publicationFactory, _assetFactory, _linkFactory);
        }

        private void UseDefaultsWhenNoImplementationProvided()
        {
            _serviceFacade = _serviceFacade ?? new S3ServiceFacade();

            _modelService = _modelService ?? _serviceFacade.CreateModelService();

            _publicationFactory = _publicationFactory ?? new PublicationFactory(_serviceFacade.CreatePublicationService());
            _assetFactory = _assetFactory ?? new AssetFactory();
            _linkFactory = _linkFactory ?? new LinkFactory();
        }
    }
}