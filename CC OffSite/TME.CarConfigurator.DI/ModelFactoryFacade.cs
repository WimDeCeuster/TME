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
        private IWheelDriveFactory _wheelDriveFactory;
        private ISteeringFactory _steeringFactory;
        private IGradeFactory _gradeFactory;
        private ICarFactory _carFactory;
        private ISubModelFactory _subModelFactory;
        private IGradeEquipmentFactory _gradeEquipmentFactory;

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
        
        public IModelFactoryFacade WithSubModelFactory(ISubModelFactory subModelFactory)
        {
            _subModelFactory = subModelFactory;

            return this;
        }

        public IModelFactoryFacade WithWheelDriveFactory(IWheelDriveFactory wheelDriveFactory)
        {
            _wheelDriveFactory = wheelDriveFactory;

            return this;
        }

        public IModelFactoryFacade WithSteeringFactory(ISteeringFactory steeringFactory)
        {
            _steeringFactory = steeringFactory;

            return this;
        }

        public IModelFactoryFacade WithGradeFactory(IGradeFactory gradeFactory)
        {
            _gradeFactory = gradeFactory;

            return this;
        }

        public IModelFactoryFacade WithCarFactory(ICarFactory carFactory)
        {
            _carFactory = carFactory;

            return this;
        }

        public IModelFactoryFacade WithGradeEquipmentFactory(IGradeEquipmentFactory gradeEquipmentFactory)
        {
            _gradeEquipmentFactory = gradeEquipmentFactory;

            return this;
        }

        public IModelFactory Create()
        {
            UseDefaultsWhenNoImplementationProvided();

            return new ModelFactory(_modelService, _publicationFactory, _bodyTypeFactory, _engineFactory, _transmissionFactory, _wheelDriveFactory, _steeringFactory, _gradeFactory, _carFactory,_subModelFactory);
        }

        private void UseDefaultsWhenNoImplementationProvided()
        {
            _serviceFacade = _serviceFacade ?? new S3ServiceFacade();

            _modelService = _modelService ?? _serviceFacade.CreateModelService();

            _publicationFactory = _publicationFactory ?? new PublicationFactory(_serviceFacade.CreatePublicationService());
            _assetFactory = _assetFactory ?? new AssetFactory(_serviceFacade.CreateAssetService());
            _bodyTypeFactory = _bodyTypeFactory ?? new BodyTypeFactory(_serviceFacade.CreateBodyTypeService(), _assetFactory);
            _engineFactory = _engineFactory ?? new EngineFactory(_serviceFacade.CreateEngineService(), _assetFactory);
            _transmissionFactory = _transmissionFactory ?? new TransmissionFactory(_serviceFacade.CreateTransmissionService(),_assetFactory);
            _wheelDriveFactory = _wheelDriveFactory ?? new WheelDriveFactory(_serviceFacade.CreateWheelDriveService(), _assetFactory);
            _steeringFactory = _steeringFactory ?? new SteeringFactory(_serviceFacade.CreateSteeringService());
            _gradeEquipmentFactory = _gradeEquipmentFactory ?? new GradeEquipmentFactory(_serviceFacade.CreateGradeEquipmentService());
            _gradeFactory = _gradeFactory ?? new GradeFactory(_serviceFacade.CreateGradeService(), _assetFactory, _gradeEquipmentFactory);
            _carFactory = _carFactory ?? new CarFactory(_serviceFacade.CreateCarService(), _bodyTypeFactory, _engineFactory);
            _subModelFactory = _subModelFactory ?? new SubModelFactory(_serviceFacade.CreateSubModelService(), _assetFactory);
        }
    }
}