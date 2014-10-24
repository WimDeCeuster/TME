using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Helpers;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.Builders
{
    public class GradeOptionPublisherBuilder
    {
        private IGradeOptionService _service = A.Fake<IGradeOptionService>();
        private ITimeFrameSubObjectPublishHelper _timeFramePublishHelper = new TimeFrameSubObjectPublishHelper();

        public GradeOptionPublisherBuilder WithService(IGradeOptionService service)
        {
            _service = service;

            return this;
        }

        public GradeOptionPublisher Build()
        {
            return new GradeOptionPublisher(_service, _timeFramePublishHelper);
        }
    }
}
