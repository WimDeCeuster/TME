using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;

using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Publisher.Interfaces;

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

        public async Task PublishGenerationGradesAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            await _timeFramePublishHelper.PublishBaseObjectList(context, timeFrame => timeFrame.Grades, _gradeService.PutTimeFrameGenerationGrades);
        }

        public async Task PublishSubModelGradesAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            await _timeFramePublishHelper.PublishPerParent(context, timeFrame => timeFrame.SubModelGrades.Keys,
                (timeFrame, subModelId) => timeFrame.SubModelGrades[subModelId], _gradeService.PutGradesPerSubModel);
        }
    }
}
