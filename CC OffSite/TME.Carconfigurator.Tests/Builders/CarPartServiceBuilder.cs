using FakeItEasy;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.Carconfigurator.Tests.Builders
{
    public class CarPartServiceBuilder
    {
        private IService _service = A.Fake<IService>();
        private ISerialiser _serialiser = A.Fake<ISerialiser>();
        private IKeyManager _keyManager = A.Fake<IKeyManager>();

        public CarPartServiceBuilder WithService(IService service)
        {
            _service = service;

            return this;
        }

        public CarPartServiceBuilder WithSerialiser(ISerialiser serialiser)
        {
            _serialiser = serialiser;

            return this;
        }

        public CarPartServiceBuilder WithKeyManager(IKeyManager keyManager)
        {
            _keyManager = keyManager;

            return this;
        }

        public CarPartService Build()
        {
            return new CarPartService(_service, _serialiser, _keyManager);
        } 
    }
}