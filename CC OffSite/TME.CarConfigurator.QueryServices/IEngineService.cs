
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryServices
{
    public interface IEngineService
    {
        IEnumerable<Engine> GetEngines(Guid publicationId, Guid publicationTimeFrameId, Context context);
    }
}
