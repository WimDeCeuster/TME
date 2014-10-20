using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.Carconfigurator.Tests.Builders
{
    public class CarServiceBuilder
    {
        private IService _service = A.Fake<IService>();
        private ISerialiser _serialiser = A.Fake<ISerialiser>();
        private IKeyManager _keyManager = A.Fake<IKeyManager>();
        
        public CarServiceBuilder WithService(IService service)
        {
            _service = service;

            return this;
        }

        public CarServiceBuilder WithSerialiser(ISerialiser serialiser)
        {
            _serialiser = serialiser;

            return this;
        }

        public CarServiceBuilder WithKeyManager(IKeyManager keyManager)
        {
            _keyManager = keyManager;

            return this;
        }

        public CarService Build()
        {
            return new CarService(_service, _serialiser, _keyManager);
        }
    }
}
