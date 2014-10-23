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
    public class GradeOptionPublisher : IGradeOptionPublisher
    {
        readonly IGradeOptionService _gradeOptionService;
        readonly ITimeFrameSubObjectPublishHelper _timeFrameSubObjectPublishHelper;

        public GradeOptionPublisher(IGradeOptionService gradeOptionService, ITimeFrameSubObjectPublishHelper timeFrameSubObjectPublishHelper)
        {
            if (gradeOptionService == null) throw new ArgumentNullException("gradeOptionService");
            if (timeFrameSubObjectPublishHelper == null) throw new ArgumentNullException("timeFrameSubObjectPublishHelper");

            _gradeOptionService = gradeOptionService;
            _timeFrameSubObjectPublishHelper = timeFrameSubObjectPublishHelper;
        }

        public async Task<IEnumerable<Result>> Publish(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            return await _timeFrameSubObjectPublishHelper.Publish(context, timeFrame => timeFrame.GradeOptions, _gradeOptionService.Put);
        }
    }
}
