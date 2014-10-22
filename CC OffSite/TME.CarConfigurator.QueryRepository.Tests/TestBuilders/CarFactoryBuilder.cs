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

        public ICarFactory Build()
        {
            return new CarFactory(_carService, _bodyTypeFactory, _engineFactory);
        }
    }
}
