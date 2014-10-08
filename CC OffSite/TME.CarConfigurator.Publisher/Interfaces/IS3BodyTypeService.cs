using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Enums.Result;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IS3BodyTypeService
    {
        Task<IEnumerable<Result>> PutGenerationBodyTypes(IContext context);
    }
}
