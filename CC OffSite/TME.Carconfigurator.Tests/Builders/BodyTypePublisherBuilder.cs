using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Helpers;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.Builders
{
    public class BodyTypePublisherBuilder
    {
        private IBodyTypeService _service = A.Fake<IBodyTypeService>();
        private ITimeFramePublishHelper _timeFramePublishHelper = new TimeFramePublishHelper();

        public BodyTypePublisherBuilder WithService(IBodyTypeService service)
        {
            _service = service;

            return this;
        }

        public BodyTypePublisher Build()
        {
            return new BodyTypePublisher(_service, _timeFramePublishHelper);
        }
    }
}
