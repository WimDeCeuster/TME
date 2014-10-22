using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Helpers;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.Builders
{
    public class EnginePublisherBuilder
    {
        private IEngineService _service = A.Fake<IEngineService>();
        private ITimeFramePublishHelper _timeFramePublishHelper = new TimeFramePublishHelper();

        public EnginePublisherBuilder WithService(IEngineService service)
        {
            _service = service;

            return this;
        }

        public EnginePublisher Build()
        {
            return new EnginePublisher(_service, _timeFramePublishHelper);
        }
    }
}
