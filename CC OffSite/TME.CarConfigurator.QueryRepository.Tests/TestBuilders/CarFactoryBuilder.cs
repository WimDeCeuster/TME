using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class CarFactoryBuilder
    {
        private ICarService _carService = A.Fake<ICarService>();
        private IBodyTypeFactory _bodyTypeFactory = A.Fake<IBodyTypeFactory>();
        private IEngineFactory _engineFactory = A.Fake<IEngineFactory>();
        private ITransmissionFactory _transmissionFactory = A.Fake<ITransmissionFactory>();
        private IWheelDriveFactory _wheelDriveFactory = A.Fake<IWheelDriveFactory>();
        private IGradeFactory _gradeFactory = A.Fake<IGradeFactory>();
        private ISubModelFactory _subModelFactory = A.Fake<ISubModelFactory>();
        private ICarPartFactory _carPartFactory = A.Fake<ICarPartFactory>();
        private IEquipmentFactory _equipmentFactory = A.Fake<IEquipmentFactory>();
        private IPackFactory _packFactory = A.Fake<IPackFactory>();

        public CarFactoryBuilder WithCarService(ICarService carService)
        {
            _carService = carService;

            return this;
        }

        public CarFactoryBuilder WithBodyTypeFactory(IBodyTypeFactory bodyTypeFactory)
        {
            _bodyTypeFactory = bodyTypeFactory;

            return this;
        }

        public CarFactoryBuilder WithEngineFactory(IEngineFactory engineFactory)
        {
            _engineFactory = engineFactory;

            return this;
        }

        public CarFactoryBuilder WithTransmissionFactory(ITransmissionFactory transmissionFactory)
        {
            _transmissionFactory = transmissionFactory;

            return this;
        }

        public CarFactoryBuilder WithWheelDriveFactory(IWheelDriveFactory wheelDriveFactory)
        {
            _wheelDriveFactory = wheelDriveFactory;
            return this;
        }

        public CarFactoryBuilder WithGradeFactory(IGradeFactory gradeFactory)
        {
            _gradeFactory = gradeFactory;
            return this;
        }

        public CarFactoryBuilder WithSubModelFactory(ISubModelFactory subModelFactory)
        {
            _subModelFactory = subModelFactory;
            return this;
        }

        public CarFactoryBuilder WithCarPartFactory(ICarPartFactory carPartFactory)
        {
            _carPartFactory = carPartFactory;
            return this;
        }
        
        public CarFactoryBuilder WithEquipmentFactory(IEquipmentFactory equipmentFactory)
        {
        	_equipmentFactory = equipmentFactory;
        	return this;
        }
        
        public CarFactoryBuilder WithPackFactory(IPackFactory packFactory)
        {
            _packFactory = packFactory;
            return this;
        }

        public ICarFactory Build()
        {
            return new CarFactory(_carService, _bodyTypeFactory, _engineFactory, _transmissionFactory, _wheelDriveFactory, _gradeFactory, _subModelFactory, _carPartFactory,_equipmentFactory,_packFactory);
        }
    }
}
