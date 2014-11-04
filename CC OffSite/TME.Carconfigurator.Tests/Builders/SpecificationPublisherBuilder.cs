using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Helpers;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.Builders
{
    public class SpecificationsPublisherBuilder
    {
        private ISpecificationsService _service = A.Fake<ISpecificationsService>();
        private readonly ITimeFramePublishHelper _timeFramePublishHelper = new TimeFramePublishHelper();

        public SpecificationsPublisherBuilder WithService(ISpecificationsService service)
        {
            _service = service;

            return this;
        }

        public SpecificationsPublisher Build()
        {
            return new SpecificationsPublisher(_service, _timeFramePublishHelper);
        }
    }
}
