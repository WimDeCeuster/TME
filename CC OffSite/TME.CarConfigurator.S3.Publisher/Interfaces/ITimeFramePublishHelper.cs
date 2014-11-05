using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;

using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.S3.Publisher.Interfaces
{
    public delegate Task PublishGenerationItem<T>(String brand, String country, Guid publicationId, Guid timeFrameId, T item);
    public delegate Task PublishGenerationSubItem<TParent, TItem>(String brand, String country, Guid publicationId, Guid timeFrameId, TParent parent, TItem item);

    public interface ITimeFramePublishHelper
    {
        Task PublishObjects<T>(IContext context, Func<TimeFrame, T> objectsGetter, PublishGenerationItem<T> publish);
        Task PublishBaseObjectList<T>(IContext context, Func<TimeFrame, IEnumerable<T>> objectsGetter, PublishGenerationItem<IEnumerable<T>> publish)
            where T : BaseObject;
        Task PublishList<T>(IContext context, Func<TimeFrame, IEnumerable<T>> objectsGetter, PublishGenerationItem<IEnumerable<T>> publish);

        Task PublishPerParent<TParent, TItem>(IContext context,
            Func<TimeFrame, IEnumerable<TParent>> parentsGetter,
            Func<TimeFrame, TParent, TItem> itemGetter,
            PublishGenerationSubItem<TParent, TItem> publish);
    }
}
