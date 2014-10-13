using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.S3.QueryServices
{
    public class EngineService : IEngineService
    {
        public IEnumerable<Repository.Objects.Engine> GetEngines(Guid publicationId, Guid publicationTimeFrameId, Repository.Objects.Context context)
        {
            throw new NotImplementedException();
        }
    }
}
