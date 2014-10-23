using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.S3.Publisher.Extensions;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher.Helpers
{
    public class TimeFrameSubObjectPublishHelper : ITimeFrameSubObjectPublishHelper
    {
        public async Task<IEnumerable<Result>> Publish<T>(IContext context,
            Func<TimeFrame, IReadOnlyDictionary<Guid, IReadOnlyList<T>>> objectsGetter,
            Func<String, String, Guid, Guid, Guid, IEnumerable<T>, Task<Result>> publish)
            where T : BaseObject
        {
            if (context == null) throw new ArgumentNullException("context");

            var tasks = new List<Task<IEnumerable<Result>>>();

            foreach (var entry in context.ContextData)
            {
                var language = entry.Key;
                var data = entry.Value;
                var timeFrames = context.TimeFrames[language];

                tasks.Add(Publish(context.Brand, context.Country, timeFrames, data.Publication.ID, objectsGetter, publish));
            }

            var result = await Task.WhenAll(tasks);
            return result.SelectMany(xs => xs);
        }

        async Task<IEnumerable<Result>> Publish<T>(
            String brand,
            String country,
            IEnumerable<TimeFrame> timeFrames,
            Guid publicationID,
            Func<TimeFrame, IReadOnlyDictionary<Guid, IReadOnlyList<T>>> objectsGetter,
            Func<String, String, Guid, Guid, Guid, IEnumerable<T>, Task<Result>> publish)
            where T : BaseObject
        {
            var tasks = timeFrames.SelectMany(timeFrame =>
            {
                var items = objectsGetter(timeFrame);
                return items.Select(entry => publish(brand, country, publicationID, timeFrame.ID, entry.Key, entry.Value.Order()));
            });

            return await Task.WhenAll(tasks);
        }
    }
}
