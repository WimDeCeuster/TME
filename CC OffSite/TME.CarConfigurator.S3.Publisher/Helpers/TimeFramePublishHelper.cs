using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IEnumerable<Result>> PublishObjects<T>(IContext context, Func<TimeFrame, T> objectsGetter, Func<String, String, Guid, Guid, T, Task<IEnumerable<Result>>> publish)
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

        public async Task<IEnumerable<Result>> PublishBaseObjectList<T>(IContext context, Func<TimeFrame, IEnumerable<T>> objectsGetter, Func<String, String, Guid, Guid, IEnumerable<T>, Task<Result>> publish)
            where T : BaseObject
        {
            if (context == null) throw new ArgumentNullException("context");

            return await PublishObjects(
                context,
                timeFrame => objectsGetter(timeFrame).Order(),
                async (brand, country, pubId, tfId, x) => await Task.WhenAll(publish(brand, country, pubId, tfId, x)));
        }

        public async Task<IEnumerable<Result>> PublishList<T>(IContext context, Func<TimeFrame, IEnumerable<T>> objectsGetter, Func<string, string, Guid, Guid, IEnumerable<T>, Task<Result>> publish)
        {
            if (context == null) throw new ArgumentNullException("context");

            return await PublishObjects(
                context,
                timeFrame => objectsGetter(timeFrame),
                async (brand, country, pubId, tfId, x) => await Task.WhenAll(publish(brand, country, pubId, tfId, x)));

        }

        async Task<IEnumerable<Result>> Publish<T>(String brand, String country, IEnumerable<TimeFrame> timeFrames, Guid publicationID, Func<TimeFrame, T> objectsGetter, Func<String, String, Guid, Guid, T, Task<IEnumerable<Result>>> publish)
        {
            var tasks = timeFrames.Select(timeFrame => publish(brand, country, publicationID, timeFrame.ID, objectsGetter(timeFrame)));

            var results = await Task.WhenAll(tasks);
            return results.SelectMany(xs => xs);
        }
    }
}
