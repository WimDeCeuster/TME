using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Helpers;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.Builders
{
    public class GradeAccessoryPublisherBuilder
    {
        private IGradeAccessoryService _service = A.Fake<IGradeAccessoryService>();
        private ITimeFrameSubObjectPublishHelper _timeFramePublishHelper = new TimeFrameSubObjectPublishHelper();

        public GradeAccessoryPublisherBuilder WithService(IGradeAccessoryService service)
        {
            _service = service;

            return this;
        }

        public GradeAccessoryPublisher Build()
        {
            return new GradeAccessoryPublisher(_service, _timeFramePublishHelper);
        }
    }
}
