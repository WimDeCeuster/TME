using System;
using TME.CarConfigurator.Configuration;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Configuration;
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
        private readonly IConfigurationManager _configurationManager;
        private IService _service;
        private IKeyManager _keyManager;
        private ISerialiser _serializer;

        private IModelService _modelService;

        public IService Service
        {
            get { return _service ?? new Service(_configurationManager.Environment, _configurationManager.DataSubset, new AmazonS3Factory()); }
        }

        public IKeyManager KeyManager
        {
            get { return _keyManager ?? new KeyManager(); }
        }

        public ISerialiser Serializer
        {
            get { return _serializer ?? new Serialiser(); }
        }

        public S3ServiceFacade()
            : this(new ConfigurationManager())
        {

        }

        public S3ServiceFacade(IConfigurationManager configurationManager)
        {
            if (configurationManager == null) throw new ArgumentNullException("configurationManager");
            _configurationManager = configurationManager;
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

        public IServiceFacade WithModelService(IModelService modelService)
        {
            _modelService = modelService;

            return this;
        }

        public IModelService CreateModelService()
        {
            return _modelService ?? new ModelService(Serializer, Service, KeyManager);
        }

        public IPublicationService CreatePublicationService()
        {
            return new PublicationService(Serializer, Service, KeyManager);
        }
    }
}