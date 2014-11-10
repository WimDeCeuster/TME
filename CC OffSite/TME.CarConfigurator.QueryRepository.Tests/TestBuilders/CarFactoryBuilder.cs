using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public ICarFactory Build()
        {
            return new CarFactory(_carService, _bodyTypeFactory, _engineFactory, _transmissionFactory, _wheelDriveFactory, _gradeFactory, _subModelFactory);
        }
    }
}
