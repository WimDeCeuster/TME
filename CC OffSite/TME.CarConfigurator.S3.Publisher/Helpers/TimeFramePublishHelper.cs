using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.S3.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.S3.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;

namespace TME.CarConfigurator.S3.Publisher.Helpers
{
    public class TimeFramePublishHelper : ITimeFramePublishHelper
    {
        public async Task<IEnumerable<Result>> PublishTimeFrameObjects<T>(IContext context,
            Func<TimeFrame, IEnumerable<T>> objectsGetter,
            Func<String, String, Guid, Guid, IEnumerable<T>, Task<Result>> publish)
            where T : BaseObject
        {
            if (context == null) throw new ArgumentNullException("context");

            var tasks = new List<Task<IEnumerable<Result>>>();

            foreach (var entry in context.ContextData)
            {
                var language = entry.Key;
                var data = entry.Value;
                var timeFrames = context.TimeFrames[language];

                tasks.Add(PublishTimeFrameObjects(context.Brand, context.Country, timeFrames, data, objectsGetter, publish));
            }

            var result = await Task.WhenAll(tasks);
            return result.SelectMany(xs => xs);
        }

        public async Task<IEnumerable<Result>> PublishTimeFrameObjects<T>(
            String brand,
            String country,
            IEnumerable<TimeFrame> timeFrames,
            ContextData data,
            Func<TimeFrame, IEnumerable<T>> objectsGetter,
            Func<String, String, Guid, Guid, IEnumerable<T>, Task<Result>> publish)
            where T : BaseObject
        {
            var tasks = timeFrames.Select(timeFrame =>
                publish(brand, country, data.Publication.ID, timeFrame.ID, objectsGetter(timeFrame).Order()));

            return await Task.WhenAll(tasks);
        }
    }
}
