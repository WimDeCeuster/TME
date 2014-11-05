using System;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;

using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher
{
    public class SteeringPublisher : ISteeringPublisher
    {
        readonly ISteeringService _steeringService;
        readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public SteeringPublisher(ISteeringService steeringService, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (steeringService == null) throw new ArgumentNullException("steeringService");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _steeringService = steeringService;
            _timeFramePublishHelper = timeFramePublishHelper;
        }

        public async Task PublishGenerationSteeringsAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            await _timeFramePublishHelper.PublishBaseObjectList(context, timeFrame => timeFrame.Steerings, _steeringService.PutTimeFrameGenerationSteerings);
        }
    }
}
