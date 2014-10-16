using TME.CarConfigurator.DI.Interfaces;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.DI
{
    public class ModelFactoryFacade : IModelFactoryFacade
    {
        private IServiceFacade _serviceFacade;
        private IModelService _modelService;
        private IPublicationFactory _publicationFactory;
        private IBodyTypeFactory _bodyTypeFactory;
        private IAssetFactory _assetFactory;
        private IEngineFactory _engineFactory;
        private ITransmissionFactory _transmissionFactory;
        private ICarFactory _carFactory;

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

        public IModelFactoryFacade WithBodyTypeFactory(IBodyTypeFactory bodyTypeFactory)
        {
            _bodyTypeFactory = bodyTypeFactory;

            return this;
        }

        public IModelFactoryFacade WithEngineFactory(IEngineFactory engineFactory)
        {
            _engineFactory = engineFactory;

            return this;
        }

        public IModelFactoryFacade WithTransmissionFactory(ITransmissionFactory transmissionFactory)
        {
            _transmissionFactory = transmissionFactory;

            return this;
        }

        public IModelFactoryFacade WithCarFactory(ICarFactory carFactory)
        {
            _carFactory = carFactory;

            return this;
        }

        public IModelFactory Create()
        {
            UseDefaultsWhenNoImplementationProvided();

            return new ModelFactory(_modelService, _publicationFactory, _bodyTypeFactory, _engineFactory, _transmissionFactory, _carFactory);
        }

        private void UseDefaultsWhenNoImplementationProvided()
        {
            _serviceFacade = _serviceFacade ?? new S3ServiceFacade();

            _modelService = _modelService ?? _serviceFacade.CreateModelService();

            _publicationFactory = _publicationFactory ?? new PublicationFactory(_serviceFacade.CreatePublicationService());
            _assetFactory = _assetFactory ?? new AssetFactory(_serviceFacade.CreateAssetService());
            _bodyTypeFactory = _bodyTypeFactory ?? new BodyTypeFactory(_serviceFacade.CreateBodyTypeService(), _assetFactory);
            _engineFactory = _engineFactory ?? new EngineFactory(_serviceFacade.CreateEngineService(), _assetFactory);
            _transmissionFactory = _transmissionFactory ?? new TransmissionFactory(_serviceFacade.CreateTransmissionService());
            _carFactory = _carFactory ?? new CarFactory(_serviceFacade.CreateCarService());
        }
    }
}