using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher
{
    public class ColourCombinationPublisher : IColourCombinationPublisher
    {
        private readonly IColourCombinationService _service;
        private readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public ColourCombinationPublisher(IColourCombinationService service, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _service = service;
            _timeFramePublishHelper = timeFramePublishHelper;
        }

        public async Task<IEnumerable<Result>> PublishGenerationColourCombinationsAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            return await _timeFramePublishHelper.PublishList(context, timeFrame => timeFrame.ColourCombinations,
                _service.PutTimeFrameGenerationColourCombinationsAsync);
        }
    }
}