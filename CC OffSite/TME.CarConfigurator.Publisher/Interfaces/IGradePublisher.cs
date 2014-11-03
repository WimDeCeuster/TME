using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IGradePublisher
    {
        Task<IEnumerable<Result>> PublishGenerationGradesAsync(IContext context);
        Task<IEnumerable<Result>>  PublishSubModelGradesAsync(IContext context);
    }
}
