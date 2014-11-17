using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Helpers;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.Builders
{
    public class PackPublisherBuilder
    {
        private IPackService _service = A.Fake<IPackService>();
        private readonly ITimeFramePublishHelper _timeFramePublishHelper = new TimeFramePublishHelper();

        public PackPublisherBuilder WithService(IPackService service)
        {
            _service = service;

            return this;
        }

        public PackPublisher Build()
        {
            return new PackPublisher(_service, _timeFramePublishHelper);
        }
    }
}