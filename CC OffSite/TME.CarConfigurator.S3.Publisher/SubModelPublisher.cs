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
    public class SubModelPublisher : ISubModelPublisher
    {
        private readonly ISubModelService _subModelService;
        readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public SubModelPublisher(ISubModelService subModelService, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (subModelService == null) throw new ArgumentNullException("subModelService");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _subModelService = subModelService;
            _timeFramePublishHelper = timeFramePublishHelper;
        }

        public async Task<IEnumerable<Result>> PublishGenerationSubModelsAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            return await _timeFramePublishHelper.PublishTimeFrameObjects(context, timeFrame => timeFrame.SubModels, _subModelService.PutTimeFrameGenerationSubModelsAsync);
        }
    }
}