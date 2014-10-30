using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.S3.Publisher.Interfaces
{
    public interface ITimeFramePublishHelper
    {
        Task<IEnumerable<Result>> PublishObjects<T>(IContext context, Func<TimeFrame, T> objectsGetter, Func<String, String, Guid, Guid, T, Task<IEnumerable<Result>>> publish);
        Task<IEnumerable<Result>> PublishBaseObjectList<T>(IContext context, Func<TimeFrame, IEnumerable<T>> objectsGetter, Func<String, String, Guid, Guid, IEnumerable<T>, Task<Result>> publish)
            where T : BaseObject;
        Task<IEnumerable<Result>> PublishList<T>(IContext context, Func<TimeFrame, IEnumerable<T>> objectsGetter, Func<String, String, Guid, Guid, IEnumerable<T>, Task<Result>> publish);

        Task<IEnumerable<Result>> PublishObjectsPerSubModel<T>(IContext context,
            Func<TimeFrame, IReadOnlyList<SubModel>> subModelGetter, Func<TimeFrame, T> objectsGetter,
            Func<String, String, Guid, Guid,Guid,List<Grade>, T, Task<IEnumerable<Result>>> publish);
    }
}
