using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.S3.Publisher;

namespace TME.Carconfigurator.Tests.Builders
{
    public class CarPublisherBuilder
    {
        private ICarService _service = A.Fake<ICarService>();

        public CarPublisherBuilder WithService(ICarService service)
        {
            _service = service;

            return this;
        }
        
        public CarPublisher Build()
        {
            return new CarPublisher(_service);
        }
    }
}
