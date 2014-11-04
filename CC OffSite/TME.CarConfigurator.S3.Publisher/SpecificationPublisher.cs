﻿using System;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;

using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher
{
    public class SpecificationsPublisher : ISpecificationsPublisher
    {
        readonly ISpecificationsService _specificationsService;
        readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public SpecificationsPublisher(ISpecificationsService specificationsService, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (specificationsService == null) throw new ArgumentNullException("specificationsService");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _specificationsService = specificationsService;
            _timeFramePublishHelper = timeFramePublishHelper;
        }

        public async Task PublishCategoriesAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            await _timeFramePublishHelper.PublishList(context, timeFrame => timeFrame.SpecificationCategories, _specificationsService.PutCategoriesAsync);
        }
    }
}
