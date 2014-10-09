using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.CommandServices
{
    public interface IBodyTypeService
    {
        Task<IEnumerable<Result>> PutGenerationBodyTypes(IContext context);
    }
}
