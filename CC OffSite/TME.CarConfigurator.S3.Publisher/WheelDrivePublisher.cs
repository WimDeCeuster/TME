using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher
{
    public class WheelDrivePublisher : IWheelDrivePublisher
    {
        readonly IWheelDriveService _wheelDriveService;
        readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public WheelDrivePublisher(IWheelDriveService wheelDriveService, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (wheelDriveService == null) throw new ArgumentNullException("wheelDriveService");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _wheelDriveService = wheelDriveService;
            _timeFramePublishHelper = timeFramePublishHelper;
        }

        public async Task<IEnumerable<Result>> PublishGenerationWheelDrivesAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            return await _timeFramePublishHelper.PublishBaseObjectList(context, timeFrame => timeFrame.WheelDrives, _wheelDriveService.PutTimeFrameGenerationWheelDrives);
        }
    }
}
