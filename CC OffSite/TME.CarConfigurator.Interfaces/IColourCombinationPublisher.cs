using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;

namespace TME.CarConfigurator.Interfaces
{
    public interface IColourCombinationPublisher  
    {
        Task<IEnumerable<Result>>  PublishGenerationColourCombinationsAsync(IContext context);
    }
}