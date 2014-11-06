using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.S3.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.S3.Publisher.Interfaces;


namespace TME.CarConfigurator.S3.Publisher.Helpers
{
    public class TimeFramePublishHelper : ITimeFramePublishHelper
    {
        private delegate Task PublishPerTimeFrame(String country, String brand, IReadOnlyList<TimeFrame> timeFrames, Guid publicationId);

        public async Task PublishObjects<T>(IContext context, Func<TimeFrame, T> objectsGetter, PublishGenerationItem<T> publish)
        {
            if (context == null) throw new ArgumentNullException("context");

            await PerTimeFrame(context, async (brand, country, timeFrames, publicationId) =>
                await Publish(brand, country, timeFrames, publicationId, objectsGetter, publish));
        }

        public async Task PublishBaseObjectList<T>(IContext context, Func<TimeFrame, IEnumerable<T>> objectsGetter, PublishGenerationItem<IEnumerable<T>> publish)
            where T : BaseObject
        {
            if (context == null) throw new ArgumentNullException("context");

            await PublishObjects(
                context,
                timeFrame => objectsGetter(timeFrame).Order(),
                async (brand, country, pubId, tfId, x) => await Task.WhenAll(publish(brand, country, pubId, tfId, x)));
        }

        public async Task PublishList<T>(IContext context, Func<TimeFrame, IEnumerable<T>> objectsGetter, PublishGenerationItem<IEnumerable<T>> publish)
        {
            if (context == null) throw new ArgumentNullException("context");

            await PublishObjects(
                context,
                objectsGetter,
                async (brand, country, pubId, tfId, x) => await Task.WhenAll(publish(brand, country, pubId, tfId, x)));
        }

        public async Task PublishPerParent<TParent, TItem>(
            IContext context,
            Func<TimeFrame, IEnumerable<TParent>> parentsGetter,
            Func<TimeFrame, TParent, TItem> itemGetter,
            PublishGenerationSubItem<TParent, TItem> publish)
        {
            if (context == null) throw new ArgumentNullException("context");

            await PerTimeFrame(context,  async (brand, country, timeFrames, publicationId) =>
                await PublishPerParent(brand, country, timeFrames, publicationId, parentsGetter, itemGetter, publish));
        }
        
        async Task PerTimeFrame(IContext context, PublishPerTimeFrame publishPerTimeFrame)
        {
            var tasks = new List<Task>();

            foreach (var entry in context.ContextData)
            {
                var language = entry.Key;
                var data = entry.Value;
                var timeFrames = context.TimeFrames[language];

                tasks.Add(publishPerTimeFrame(context.Brand, context.Country, timeFrames, data.Publication.ID));
            }

            await Task.WhenAll(tasks);
        }

        async Task Publish<T>(String brand, String country, IEnumerable<TimeFrame> timeFrames, Guid publicationID, Func<TimeFrame, T> objectsGetter, PublishGenerationItem<T> publish)
        {
            var tasks = timeFrames.Select(timeFrame => publish(brand, country, publicationID, timeFrame.ID, objectsGetter(timeFrame)));

            await Task.WhenAll(tasks);
        }

        async Task PublishPerParent<TParent, TItem>(String brand, String country, IEnumerable<TimeFrame> timeFrames, Guid publicationID,
            Func<TimeFrame, IEnumerable<TParent>> parentsGetter,
            Func<TimeFrame, TParent, TItem> itemGetter,
            PublishGenerationSubItem<TParent, TItem> publish)
        {
            var tasks = new List<Task>();

            foreach (var timeFrame in timeFrames)
            {
                var parents = parentsGetter(timeFrame);
              
                tasks.AddRange(parents.Select(parent => publish(brand, country, publicationID, timeFrame.ID, parent, itemGetter(timeFrame, parent))));
            }

            await Task.WhenAll(tasks);
        }
    }
}
