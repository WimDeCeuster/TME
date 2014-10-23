using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.Publisher.Extensions;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher
{
    public class CarPublisher : ICarPublisher
    {
        readonly ICarService _carService;
        readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public CarPublisher(ICarService carService, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (carService == null) throw new ArgumentNullException("carService");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _carService = carService;
            _timeFramePublishHelper = timeFramePublishHelper;
        }

        public async Task<IEnumerable<Result>> PublishGenerationCars(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            return await _timeFramePublishHelper.Publish(context, timeFrame => timeFrame.Cars, _carService.PutTimeFrameGenerationCars);
        }
    }
}
