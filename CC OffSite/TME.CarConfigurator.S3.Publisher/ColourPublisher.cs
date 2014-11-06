using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;

using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher
{
    public class ColourPublisher : IColourPublisher
    {
        private readonly IColourService _service;
        private readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public ColourPublisher(IColourService service, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _service = service;
            _timeFramePublishHelper = timeFramePublishHelper;
        }

        public async Task PublishGenerationColourCombinations(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            await _timeFramePublishHelper.PublishList(context, timeFrame => timeFrame.ColourCombinations, _service.PutTimeFrameGenerationColourCombinations);
        }
    }
}