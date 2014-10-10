using TME.CarConfigurator.Interfaces.Facades;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.S3.QueryServices;
using TME.CarConfigurator.S3.Shared;
using TME.CarConfigurator.S3.Shared.Factories;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.Facades
{
    public class S3ServiceFacade : IServiceFacade
    {
        private IService _service;
        private IKeyManager _keyManager;
        private ISerialiser _serializer;

        private IModelService _modelService;

        public S3ServiceFacade()
        {
            var amazonS3Factory = new AmazonS3Factory();


            _service = new Service("alsofake", "ffake", "fake", "fake", amazonS3Factory); // TODO: initialize with correct values, from config
            _keyManager = new KeyManager();
            _serializer = new Serialiser();
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

        public S3ServiceFacade WithKeyMangere(IKeyManager keyManager)
        {
            _keyManager = keyManager;

            return this;
        }

        public IModelService CreateModelService()
        {
            return _modelService ?? new ModelService(_serializer, _service, _keyManager);
        }

        public IPublicationService CreatePublicationService()
        {
            return new PublicationService(_serializer, _service, _keyManager);
        }

        public IServiceFacade WithModelService(IModelService modelService)
        {
            _modelService = modelService;

            return this;
        }
    }
}