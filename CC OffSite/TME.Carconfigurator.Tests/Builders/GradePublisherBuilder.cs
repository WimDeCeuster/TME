using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Helpers;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.Builders
{
    public class GradePublisherBuilder
    {
        private IGradeService _service = A.Fake<IGradeService>();
        private ITimeFramePublishHelper _timeFramePublishHelper = new TimeFramePublishHelper();

        public GradePublisherBuilder WithService(IGradeService service)
        {
            _service = service;

            return this;
        }

        public GradePublisher Build()
        {
            return new GradePublisher(_service, _timeFramePublishHelper);
        }
    }
}
