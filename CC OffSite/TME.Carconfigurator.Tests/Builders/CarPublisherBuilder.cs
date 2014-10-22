using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Helpers;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.Builders
{
    public class CarPublisherBuilder
    {
        private ICarService _service = A.Fake<ICarService>();
        private ITimeFramePublishHelper _timeFramePublishHelper = new TimeFramePublishHelper();

        public CarPublisherBuilder WithService(ICarService service)
        {
            _service = service;

            return this;
        }
        
        public CarPublisher Build()
        {
            return new CarPublisher(_service, _timeFramePublishHelper);
        }
    }
}
