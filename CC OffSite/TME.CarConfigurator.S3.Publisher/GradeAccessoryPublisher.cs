using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.S3.Publisher.Interfaces;
using TME.CarConfigurator.S3.Publisher.Extensions;

namespace TME.CarConfigurator.S3.Publisher
{
    public class GradeAccessoryPublisher : IGradeAccessoryPublisher
    {
        readonly IGradeAccessoryService _gradeAccessoryService;
        readonly ITimeFrameSubObjectPublishHelper _timeFrameSubObjectPublishHelper;

        public GradeAccessoryPublisher(IGradeAccessoryService gradeAccessoryService, ITimeFrameSubObjectPublishHelper timeFrameSubObjectPublishHelper)
        {
            if (gradeAccessoryService == null) throw new ArgumentNullException("gradeAccessoryService");
            if (timeFrameSubObjectPublishHelper == null) throw new ArgumentNullException("timeFrameSubObjectPublishHelper");

            _gradeAccessoryService = gradeAccessoryService;
            _timeFrameSubObjectPublishHelper = timeFrameSubObjectPublishHelper;
        }

        public async Task<IEnumerable<Result>> Publish(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var tasks = new List<Task<IEnumerable<Result>>>();

            foreach (var entry in context.ContextData)
            {
                var language = entry.Key;
                var data = entry.Value;
                var timeFrames = context.TimeFrames[language];

                tasks.Add(PublishTimeFrameObjects(context.Brand, context.Country, timeFrames, data.Publication.ID, timeFrame => timeFrame.GradeAccessories, _gradeAccessoryService.Put));
            }

            var result = await Task.WhenAll(tasks);
            return result.SelectMany(xs => xs);
        }

        async Task<IEnumerable<Result>> PublishTimeFrameObjects<T>(
            String brand,
            String country,
            IEnumerable<TimeFrame> timeFrames,
            Guid publicationID,
            Func<TimeFrame, IReadOnlyDictionary<Guid, IReadOnlyList<T>>> objectsGetter,
            Func<String, String, Guid, Guid, Guid, IEnumerable<T>, Task<Result>> publish)
            where T : BaseObject
        {
            var tasks = timeFrames.SelectMany(timeFrame => {
                    var items = objectsGetter(timeFrame);
                    return items.Select(entry => publish(brand, country, publicationID, timeFrame.ID, entry.Key, entry.Value.Order()));
                });

            return await Task.WhenAll(tasks);
        }
    }
}
