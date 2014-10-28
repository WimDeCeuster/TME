using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher
{
    public class GradePackPublisher : IGradePackPublisher
    {
        public GradePackPublisher(IGradePackService service, ITimeFramePublishHelper timeFramePublishHelper)
        {
            
        }

        public Task<IEnumerable<Result>> PublishAsync(IContext context)
        {
            throw new NotImplementedException();
        }
    }
}