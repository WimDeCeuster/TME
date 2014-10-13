using TME.CarConfigurator.Configuration;
using TME.CarConfigurator.DI.Interfaces;
using TME.CarConfigurator.Interfaces.Configuration;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.S3.QueryServices;
using TME.CarConfigurator.S3.Shared;
using TME.CarConfigurator.S3.Shared.Factories;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.DI
{
    public class S3ServiceFacade : IServiceFacade
    {
        private IConfigurationManager _configurationManager = new ConfigurationManager();
        private IService _service;
        private IKeyManager _keyManager;
        private ISerialiser _serializer;

        private IModelService _modelService;
        private IPublicationService _publicationService;
        private IBodyTypeService _bodyTypeService;
        private IEngineService _engineService;

        public IService Service
        {
            get { return _service ?? new Service(_configurationManager.Environment, _configurationManager.DataSubset, _configurationManager.AmazonAccessKeyId, _configurationManager.AmazonSecretAccessKey, new AmazonS3Factory()); }
        }

        public IKeyManager KeyManager
        {
            get { return _keyManager ?? new KeyManager(); }
        }

        public ISerialiser Serializer
        {
            get { return _serializer ?? new Serialiser(); }
        }

        public S3ServiceFacade WithConfigurationManager(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;

            return this;
        }

        public S3ServiceFacade WithService(IService service)
        {
            _service = service;

            return this;
        }

        public S3ServiceFacade WithSerializer(ISerialiser serializer)
        {
            _serializer = serializer;

            return this;
        }

        public S3ServiceFacade WithKeyManager(IKeyManager keyManager)
        {
            _keyManager = keyManager;

            return this;
        }

        public IServiceFacade WithModelService(IModelService modelService)
        {
            _modelService = modelService;

            return this;
        }

        public IServiceFacade WithPublicationService(IPublicationService publicationService)
        {
            _publicationService = publicationService;

            return this;
        }

        public IServiceFacade WithEngineService(IEngineService engineService)
        {
            _engineService = engineService;

            return this;
        }

        public IServiceFacade WithBodyTypeService(IBodyTypeService bodyTypeService)
        {
            _bodyTypeService = bodyTypeService;

            return this;
        }

        public IModelService CreateModelService()
        {
            return _modelService ?? new ModelService(Serializer, Service, KeyManager);
        }

        public IPublicationService CreatePublicationService()
        {
            return _publicationService ?? new PublicationService(Serializer, Service, KeyManager);
        }

        public IBodyTypeService CreateBodyTypeService()
        {
            return _bodyTypeService ?? new BodyTypeService(Serializer, Service, KeyManager);
        }

        public IEngineService CreateEngineService()
        {
            return _engineService ?? new EngineService(Serializer, Service, KeyManager);
        }


    }
}