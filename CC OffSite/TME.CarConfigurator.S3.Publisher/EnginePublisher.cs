﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;

using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher
{
    public class EnginePublisher : IEnginePublisher
    {
        readonly IEngineService _engineService;
        readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public EnginePublisher(IEngineService engineService, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (engineService == null) throw new ArgumentNullException("engineService");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _engineService = engineService;
            _timeFramePublishHelper = timeFramePublishHelper;
        }

        public async Task PublishGenerationEnginesAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            await _timeFramePublishHelper.PublishBaseObjectList(context, timeFrame => timeFrame.Engines, _engineService.PutTimeFrameGenerationEngines);
        }
    }
}
