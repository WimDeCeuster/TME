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
    public class BodyTypePublisher : IBodyTypePublisher
    {
        readonly IBodyTypeService _bodyTypeService;
        readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public BodyTypePublisher(IBodyTypeService bodyTypeService, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (bodyTypeService == null) throw new ArgumentNullException("bodyTypeService");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _bodyTypeService = bodyTypeService;
            _timeFramePublishHelper = timeFramePublishHelper;
        }

        public async Task<IEnumerable<Result>> PublishGenerationBodyTypes(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            return await _timeFramePublishHelper.PublishTimeFrameObjects(context, timeFrame => timeFrame.BodyTypes, _bodyTypeService.PutTimeFrameGenerationBodyTypes);
        }
    }
}
