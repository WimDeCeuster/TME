using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.Publisher.Interfaces;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.S3.Publisher
{
    public class TransmissionPublisher : ITransmissionPublisher
    {
        readonly ITransmissionService _transmissionService;
        readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public TransmissionPublisher(ITransmissionService transmissionService, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (transmissionService == null) throw new ArgumentNullException("transmissionService");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _transmissionService = transmissionService;
            _timeFramePublishHelper = timeFramePublishHelper;
        }

        public async Task<IEnumerable<Result>> PublishGenerationTransmissions(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            return await _timeFramePublishHelper.PublishTimeFrameObjects(context, timeFrame => timeFrame.Transmissions, _transmissionService.PutTimeFrameGenerationTransmissions);
        }
    }
}
