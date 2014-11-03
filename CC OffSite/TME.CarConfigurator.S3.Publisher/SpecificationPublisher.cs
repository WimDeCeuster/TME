using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.TechnicalSpecifications;
using TME.CarConfigurator.S3.Publisher.Extensions;
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

        public async Task<IEnumerable<Result>> PublishCategoriesAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            return await _timeFramePublishHelper.PublishList(context, timeFrame => timeFrame.SpecificationCategories, _specificationsService.PutCategoriesAsync);
        }
    }
}
