using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Helpers;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.Builders
{
    public class GradePacksPublisherBuilder
    {
        private IGradePackService _service = A.Fake<IGradePackService>();
        private readonly ITimeFramePublishHelper _timeFramePublishHelper = new TimeFramePublishHelper();

        public GradePacksPublisherBuilder WithService(IGradePackService service)
        {
            _service = service;

            return this;
        }

        public GradePackPublisher Build()
        {
            return new GradePackPublisher(_service, _timeFramePublishHelper);
        }
    }
}