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
    public class GradePublisher : IGradePublisher
    {
        readonly IGradeService _gradeService;
        readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public GradePublisher(IGradeService gradeService, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (gradeService == null) throw new ArgumentNullException("gradeService");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _gradeService = gradeService;
            _timeFramePublishHelper = timeFramePublishHelper;
        }

        public async Task<IEnumerable<Result>> PublishGenerationGrades(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            return await _timeFramePublishHelper.PublishTimeFrameObjects(context, timeFrame => timeFrame.Grades, _gradeService.PutTimeFrameGenerationGrades);
        }
    }
}
