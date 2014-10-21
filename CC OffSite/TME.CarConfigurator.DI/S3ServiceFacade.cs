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
        private IAssetService _assetService;
        private IEngineService _engineService;
        private ITransmissionService _transmissionService;
        private IWheelDriveService _wheelDriveService;
        private ISteeringService _steeringService;
        private IGradeService _gradeService;
        private ICarService _carService;

        public IService Service
        {
            get { return _service ?? new Service(_configurationManager.BucketNameTemplate, _configurationManager.AmazonAccessKeyId, _configurationManager.AmazonSecretAccessKey, new AmazonS3Factory()); }
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

        public IServiceFacade WithTransmissionService(ITransmissionService transmissionService)
        {
            _transmissionService = transmissionService;

            return this;
        }

        public IServiceFacade WithWheelDriveService(IWheelDriveService wheelDriveService)
        {
            _wheelDriveService = wheelDriveService;

            return this;
        }

        public IServiceFacade WithSteeringService(ISteeringService steeringService)
        {
            _steeringService = steeringService;

            return this;
        }

        public IServiceFacade WithGradeService(IGradeService gradeService)
        {
            _gradeService = gradeService;

            return this;
        }

        public IServiceFacade WithCarService(ICarService carService)
        {
            _carService = carService;

            return this;
        }

        public IServiceFacade WithBodyTypeService(IBodyTypeService bodyTypeService)
        {
            _bodyTypeService = bodyTypeService;

            return this;
        }

        public IServiceFacade WithAssetService(IAssetService assetService)
        {
            _assetService = assetService;

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

        public IAssetService CreateAssetService()
        {
            return _assetService ?? new AssetService(Serializer, Service, KeyManager);
        }

        public IEngineService CreateEngineService()
        {
            return _engineService ?? new EngineService(Serializer, Service, KeyManager);
        }

        public ITransmissionService CreateTransmissionService()
        {
            return _transmissionService ?? new TransmissionService(Serializer, Service, KeyManager);
        }

        public IWheelDriveService CreateWheelDriveService()
        {
            return _wheelDriveService ?? new WheelDriveService(Serializer, Service, KeyManager);
        }

        public ISteeringService CreateSteeringService()
        {
            return _steeringService ?? new SteeringService(Serializer, Service, KeyManager);
        }

        public IGradeService CreateGradeService()
        {
            return _gradeService ?? new GradeService(Serializer, Service, KeyManager);
        }

        public ICarService CreateCarService()
        {
            return _carService ?? new CarService(Serializer, Service, KeyManager);
        }
    }
}