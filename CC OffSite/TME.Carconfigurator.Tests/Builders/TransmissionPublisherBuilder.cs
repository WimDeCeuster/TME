using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Helpers;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.Builders
{
    public class TransmissionPublisherBuilder
    {
        private ITransmissionService _service = A.Fake<ITransmissionService>();
        private ITimeFramePublishHelper _timeFramePublishHelper = new TimeFramePublishHelper();

        public TransmissionPublisherBuilder WithService(ITransmissionService service)
        {
            _service = service;

            return this;
        }

        public TransmissionPublisher Build()
        {
            return new TransmissionPublisher(_service, _timeFramePublishHelper);
        }
    }
}
