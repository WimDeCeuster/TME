using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Helpers;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.Builders
{
    public class ColourPublisherBuilder
    {
        private readonly ITimeFramePublishHelper _timeFramePublishHelper = new TimeFramePublishHelper();
        private IColourService _service;

        public ColourPublisherBuilder WithService(ColourService colourService)
        {
            _service = colourService;
            return this;
        }

        public IColourPublisher Build()
        {
            return new ColourPublisher(_service, _timeFramePublishHelper);
        }
    }
}