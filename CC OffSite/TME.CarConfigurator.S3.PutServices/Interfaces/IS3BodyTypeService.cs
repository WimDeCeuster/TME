using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.S3.PutServices.Interfaces
{
    public interface IS3BodyTypeService
    {
        Task<IEnumerable<Result>> PutGenerationBodyTypes(IContext context);
    }
}
