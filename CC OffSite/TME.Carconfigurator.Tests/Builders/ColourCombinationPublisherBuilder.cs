using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Helpers;
using TME.CarConfigurator.S3.Publisher.Interfaces;
using TME.Carconfigurator.Tests.GivenAS3ColourCombinationPublisher;

namespace TME.Carconfigurator.Tests.Builders
{
    public class ColourCombinationPublisherBuilder
    {
        private readonly ITimeFramePublishHelper _timeFramePublishHelper = new TimeFramePublishHelper();
        private IColourCombinationService _service;

        public ColourCombinationPublisherBuilder WithService(ColourCombinationService colourCombinationService)
        {
            _service = colourCombinationService;
            return this;
        }

        public IColourCombinationPublisher Build()
        {
            return new ColourCombinationPublisher(_service, _timeFramePublishHelper);
        }
    }
}