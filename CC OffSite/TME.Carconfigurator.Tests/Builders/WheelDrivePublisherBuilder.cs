using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Helpers;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.Builders
{
    public class WheelDrivePublisherBuilder
    {
        private IWheelDriveService _service = A.Fake<IWheelDriveService>();
        private ITimeFramePublishHelper _timeFramePublishHelper = new TimeFramePublishHelper();

        public WheelDrivePublisherBuilder WithService(IWheelDriveService service)
        {
            _service = service;

            return this;
        }

        public WheelDrivePublisher Build()
        {
            return new WheelDrivePublisher(_service, _timeFramePublishHelper);
        }
    }
}
